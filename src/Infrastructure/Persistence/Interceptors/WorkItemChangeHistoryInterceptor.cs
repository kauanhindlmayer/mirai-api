using Application.Abstractions.Authentication;
using Domain.Sprints;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
/// Diffs the tracked <see cref="WorkItem"/> graph for scalar, complex, and structural
/// (tags/attachments/links/pull-request-links) changes and stages one <see cref="WorkItemChangeSet"/>
/// per affected work item, before <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> persists
/// the underlying changes - reference-name lookups (e.g. a sprint's name) and link-target lookups are
/// read from the database as it stood before this save.
/// </summary>
internal sealed class WorkItemChangeHistoryInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    private const string TagWorkItemJoinEntityName = "TagWorkItem";

    private static readonly Dictionary<string, ScalarField> ScalarFields = new()
    {
        [nameof(WorkItem.Title)] = new("Title"),
        [nameof(WorkItem.Description)] = new("Description"),
        [nameof(WorkItem.AcceptanceCriteria)] = new("Acceptance Criteria"),
        [nameof(WorkItem.Type)] = new("Type"),
        [nameof(WorkItem.Status)] = new("Status"),
        [nameof(WorkItem.AssigneeId)] = new("Assignee", ResolveUserNameAsync),
        [nameof(WorkItem.AssignedTeamId)] = new("Assigned Team", ResolveTeamNameAsync),
        [nameof(WorkItem.ParentWorkItemId)] = new("Parent Work Item", ResolveWorkItemTitleAsync),
        [nameof(WorkItem.SprintId)] = new("Sprint", ResolveSprintNameAsync),
    };

    private static readonly Dictionary<string, string> ComplexFields = new()
    {
        [nameof(Planning.StoryPoints)] = "Story Points",
        [nameof(Planning.Priority)] = "Priority",
        [nameof(Classification.ValueArea)] = "Value Area",
    };

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await RecordAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task RecordAsync(DbContext context, CancellationToken cancellationToken)
    {
        var excludedWorkItemIds = context.ChangeTracker.Entries<WorkItem>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Select(entry => entry.Entity.Id)
            .ToHashSet();
        var state = new CaptureState(context, [], excludedWorkItemIds);

        await CaptureFieldChangesAsync(state, cancellationToken);
        await CaptureTagChangesAsync(state, cancellationToken);
        await CaptureLinkChangesAsync(state, cancellationToken);
        CaptureAttachmentChanges(state);
        CapturePullRequestLinkChanges(state);

        foreach (var (workItemId, changes) in state.ChangesByWorkItemId)
        {
            var changeSet = new WorkItemChangeSet(workItemId, userContext.UserId);
            foreach (var change in changes)
            {
                changeSet.AddChange(change.Field, change.OldValue, change.NewValue);
            }

            context.Set<WorkItemChangeSet>().Add(changeSet);
        }
    }

    private static async Task CaptureFieldChangesAsync(CaptureState state, CancellationToken cancellationToken)
    {
        var modifiedWorkItems = state.Context.ChangeTracker.Entries<WorkItem>()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedWorkItems)
        {
            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                if (!ScalarFields.TryGetValue(property.Metadata.Name, out var field))
                {
                    continue;
                }

                var oldValue = await field.FormatAsync(state.Context, property.OriginalValue, cancellationToken);
                var newValue = await field.FormatAsync(state.Context, property.CurrentValue, cancellationToken);
                AddIfChanged(state.ChangesByWorkItemId, entry.Entity.Id, field.DisplayName, oldValue, newValue);
            }

            var complexProperties = entry.ComplexProperties.SelectMany(cp => cp.Properties).Where(p => p.IsModified);
            foreach (var property in complexProperties)
            {
                if (!ComplexFields.TryGetValue(property.Metadata.Name, out var displayName))
                {
                    continue;
                }

                AddIfChanged(
                    state.ChangesByWorkItemId,
                    entry.Entity.Id,
                    displayName,
                    property.OriginalValue?.ToString(),
                    property.CurrentValue?.ToString());
            }
        }
    }

    private static async Task CaptureTagChangesAsync(CaptureState state, CancellationToken cancellationToken)
    {
        var tagJoinEntries = state.Context.ChangeTracker.Entries()
            .Where(entry => entry.Metadata.Name == TagWorkItemJoinEntityName)
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !state.ExcludedWorkItemIds.Contains((Guid)entry.Property("WorkItemsId").CurrentValue!))
            .ToList();

        if (tagJoinEntries.Count == 0)
        {
            return;
        }

        var tagIds = tagJoinEntries.Select(entry => (Guid)entry.Property("TagsId").CurrentValue!).ToHashSet();
        var tagNames = await state.Context.Set<Tag>()
            .Where(tag => tagIds.Contains(tag.Id))
            .ToDictionaryAsync(tag => tag.Id, tag => tag.Name, cancellationToken);

        foreach (var entry in tagJoinEntries)
        {
            var workItemId = (Guid)entry.Property("WorkItemsId").CurrentValue!;
            var tagId = (Guid)entry.Property("TagsId").CurrentValue!;
            var tagName = tagNames.GetValueOrDefault(tagId, "Unknown");
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, tagName);

            AddChange(state.ChangesByWorkItemId, workItemId, "Tag", oldValue, newValue);
        }
    }

    private static async Task CaptureLinkChangesAsync(CaptureState state, CancellationToken cancellationToken)
    {
        var linkEntries = state.Context.ChangeTracker.Entries<WorkItemLink>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !state.ExcludedWorkItemIds.Contains(entry.Entity.SourceWorkItemId))
            .ToList();

        if (linkEntries.Count == 0)
        {
            return;
        }

        var targetIds = linkEntries.Select(entry => entry.Entity.TargetWorkItemId).ToHashSet();
        var targetTitles = await state.Context.Set<WorkItem>()
            .Where(workItem => targetIds.Contains(workItem.Id))
            .ToDictionaryAsync(workItem => workItem.Id, workItem => workItem.Title, cancellationToken);

        foreach (var entry in linkEntries)
        {
            var link = entry.Entity;
            var targetTitle = targetTitles.GetValueOrDefault(link.TargetWorkItemId, "Unknown");
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, $"{link.LinkType} \"{targetTitle}\"");

            AddChange(state.ChangesByWorkItemId, link.SourceWorkItemId, "Link", oldValue, newValue);
        }
    }

    private static void CaptureAttachmentChanges(CaptureState state)
    {
        var attachmentEntries = state.Context.ChangeTracker.Entries<WorkItemAttachment>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !state.ExcludedWorkItemIds.Contains(entry.Entity.WorkItemId));

        foreach (var entry in attachmentEntries)
        {
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, entry.Entity.FileName);
            AddChange(state.ChangesByWorkItemId, entry.Entity.WorkItemId, "Attachment", oldValue, newValue);
        }
    }

    private static void CapturePullRequestLinkChanges(CaptureState state)
    {
        var pullRequestEntries = state.Context.ChangeTracker.Entries<WorkItemPullRequestLink>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !state.ExcludedWorkItemIds.Contains(entry.Entity.WorkItemId));

        foreach (var entry in pullRequestEntries)
        {
            var description = $"#{entry.Entity.PullRequestNumber} {entry.Entity.Title}";
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, description);
            AddChange(state.ChangesByWorkItemId, entry.Entity.WorkItemId, "Pull Request", oldValue, newValue);
        }
    }

    private static (string? OldValue, string? NewValue) DescribeAddOrRemove(EntityState state, string description) =>
        state == EntityState.Added ? (null, description) : (description, null);

    private static void AddChange(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        Guid workItemId,
        string field,
        string? oldValue,
        string? newValue)
    {
        if (!changesByWorkItemId.TryGetValue(workItemId, out var changes))
        {
            changes = [];
            changesByWorkItemId[workItemId] = changes;
        }

        changes.Add(new FieldChange(field, oldValue, newValue));
    }

    private static void AddIfChanged(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        Guid workItemId,
        string field,
        string? oldValue,
        string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        AddChange(changesByWorkItemId, workItemId, field, oldValue, newValue);
    }

    private static Task<string?> ResolveUserNameAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<User>()
            .Where(user => user.Id == id)
            .Select(user => user.FullName)
            .FirstOrDefaultAsync(cancellationToken);

    private static Task<string?> ResolveTeamNameAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<Team>()
            .Where(team => team.Id == id)
            .Select(team => team.Name)
            .FirstOrDefaultAsync(cancellationToken);

    private static Task<string?> ResolveWorkItemTitleAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<WorkItem>()
            .Where(workItem => workItem.Id == id)
            .Select(workItem => workItem.Title)
            .FirstOrDefaultAsync(cancellationToken);

    private static Task<string?> ResolveSprintNameAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<Sprint>()
            .Where(sprint => sprint.Id == id)
            .Select(sprint => sprint.Name)
            .FirstOrDefaultAsync(cancellationToken);

    private sealed record CaptureState(
        DbContext Context,
        Dictionary<Guid, List<FieldChange>> ChangesByWorkItemId,
        HashSet<Guid> ExcludedWorkItemIds);

    private sealed record FieldChange(string Field, string? OldValue, string? NewValue);

    private sealed record ScalarField(
        string DisplayName,
        Func<DbContext, Guid, CancellationToken, Task<string?>>? ResolveReferenceAsync = null)
    {
        public Task<string?> FormatAsync(DbContext context, object? value, CancellationToken cancellationToken)
        {
            if (value is null)
            {
                return Task.FromResult<string?>(null);
            }

            return ResolveReferenceAsync is null
                ? Task.FromResult(value.ToString())
                : ResolveReferenceAsync(context, (Guid)value, cancellationToken);
        }
    }
}
