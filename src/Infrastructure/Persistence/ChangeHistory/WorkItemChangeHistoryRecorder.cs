using Domain.Sprints;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.ChangeHistory;

/// <summary>
/// Diffs the tracked <see cref="WorkItem"/> graph for scalar, complex, and structural
/// (tags/attachments/links/pull-request-links) changes and stages one <see cref="WorkItemChangeSet"/>
/// per affected work item. Must run before <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>
/// persists the underlying changes, since reference-name lookups (e.g. a sprint's name) and
/// link-target lookups are read from the database as it stood before this save.
/// </summary>
internal sealed class WorkItemChangeHistoryRecorder(DbContext context)
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

    public async Task RecordAsync(Guid? changedByUserId, CancellationToken cancellationToken)
    {
        var changesByWorkItemId = new Dictionary<Guid, List<FieldChange>>();
        var excludedWorkItemIds = context.ChangeTracker.Entries<WorkItem>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Select(entry => entry.Entity.Id)
            .ToHashSet();

        await CaptureFieldChangesAsync(changesByWorkItemId, cancellationToken);
        await CaptureTagChangesAsync(changesByWorkItemId, excludedWorkItemIds, cancellationToken);
        await CaptureLinkChangesAsync(changesByWorkItemId, excludedWorkItemIds, cancellationToken);
        CaptureAttachmentChanges(changesByWorkItemId, excludedWorkItemIds);
        CapturePullRequestLinkChanges(changesByWorkItemId, excludedWorkItemIds);

        foreach (var (workItemId, changes) in changesByWorkItemId)
        {
            var changeSet = new WorkItemChangeSet(workItemId, changedByUserId);
            foreach (var change in changes)
            {
                changeSet.AddChange(change.Field, change.OldValue, change.NewValue);
            }

            context.Set<WorkItemChangeSet>().Add(changeSet);
        }
    }

    private async Task CaptureFieldChangesAsync(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        CancellationToken cancellationToken)
    {
        var modifiedWorkItems = context.ChangeTracker.Entries<WorkItem>()
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

                var oldValue = await field.FormatAsync(context, property.OriginalValue, cancellationToken);
                var newValue = await field.FormatAsync(context, property.CurrentValue, cancellationToken);
                AddIfChanged(changesByWorkItemId, entry.Entity.Id, field.DisplayName, oldValue, newValue);
            }

            var complexProperties = entry.ComplexProperties.SelectMany(cp => cp.Properties).Where(p => p.IsModified);
            foreach (var property in complexProperties)
            {
                if (!ComplexFields.TryGetValue(property.Metadata.Name, out var displayName))
                {
                    continue;
                }

                AddIfChanged(
                    changesByWorkItemId,
                    entry.Entity.Id,
                    displayName,
                    property.OriginalValue?.ToString(),
                    property.CurrentValue?.ToString());
            }
        }
    }

    private async Task CaptureTagChangesAsync(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        HashSet<Guid> excludedWorkItemIds,
        CancellationToken cancellationToken)
    {
        var tagJoinEntries = context.ChangeTracker.Entries()
            .Where(entry => entry.Metadata.Name == TagWorkItemJoinEntityName)
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !excludedWorkItemIds.Contains((Guid)entry.Property("WorkItemsId").CurrentValue!))
            .ToList();

        if (tagJoinEntries.Count == 0)
        {
            return;
        }

        var tagIds = tagJoinEntries.Select(entry => (Guid)entry.Property("TagsId").CurrentValue!).ToHashSet();
        var tagNames = await context.Set<Tag>()
            .Where(tag => tagIds.Contains(tag.Id))
            .ToDictionaryAsync(tag => tag.Id, tag => tag.Name, cancellationToken);

        foreach (var entry in tagJoinEntries)
        {
            var workItemId = (Guid)entry.Property("WorkItemsId").CurrentValue!;
            var tagId = (Guid)entry.Property("TagsId").CurrentValue!;
            var tagName = tagNames.GetValueOrDefault(tagId, "Unknown");
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, tagName);

            AddChange(changesByWorkItemId, workItemId, "Tag", oldValue, newValue);
        }
    }

    private async Task CaptureLinkChangesAsync(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        HashSet<Guid> excludedWorkItemIds,
        CancellationToken cancellationToken)
    {
        var linkEntries = context.ChangeTracker.Entries<WorkItemLink>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !excludedWorkItemIds.Contains(entry.Entity.SourceWorkItemId))
            .ToList();

        if (linkEntries.Count == 0)
        {
            return;
        }

        var targetIds = linkEntries.Select(entry => entry.Entity.TargetWorkItemId).ToHashSet();
        var targetTitles = await context.Set<WorkItem>()
            .Where(workItem => targetIds.Contains(workItem.Id))
            .ToDictionaryAsync(workItem => workItem.Id, workItem => workItem.Title, cancellationToken);

        foreach (var entry in linkEntries)
        {
            var link = entry.Entity;
            var targetTitle = targetTitles.GetValueOrDefault(link.TargetWorkItemId, "Unknown");
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, $"{link.LinkType} \"{targetTitle}\"");

            AddChange(changesByWorkItemId, link.SourceWorkItemId, "Link", oldValue, newValue);
        }
    }

    private void CaptureAttachmentChanges(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        HashSet<Guid> excludedWorkItemIds)
    {
        var attachmentEntries = context.ChangeTracker.Entries<WorkItemAttachment>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !excludedWorkItemIds.Contains(entry.Entity.WorkItemId));

        foreach (var entry in attachmentEntries)
        {
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, entry.Entity.FileName);
            AddChange(changesByWorkItemId, entry.Entity.WorkItemId, "Attachment", oldValue, newValue);
        }
    }

    private void CapturePullRequestLinkChanges(
        Dictionary<Guid, List<FieldChange>> changesByWorkItemId,
        HashSet<Guid> excludedWorkItemIds)
    {
        var pullRequestEntries = context.ChangeTracker.Entries<WorkItemPullRequestLink>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted)
            .Where(entry => !excludedWorkItemIds.Contains(entry.Entity.WorkItemId));

        foreach (var entry in pullRequestEntries)
        {
            var description = $"#{entry.Entity.PullRequestNumber} {entry.Entity.Title}";
            var (oldValue, newValue) = DescribeAddOrRemove(entry.State, description);
            AddChange(changesByWorkItemId, entry.Entity.WorkItemId, "Pull Request", oldValue, newValue);
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
            .FirstOrDefaultAsync(cancellationToken)!;

    private static Task<string?> ResolveTeamNameAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<Team>()
            .Where(team => team.Id == id)
            .Select(team => team.Name)
            .FirstOrDefaultAsync(cancellationToken)!;

    private static Task<string?> ResolveWorkItemTitleAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<WorkItem>()
            .Where(workItem => workItem.Id == id)
            .Select(workItem => workItem.Title)
            .FirstOrDefaultAsync(cancellationToken)!;

    private static Task<string?> ResolveSprintNameAsync(DbContext context, Guid id, CancellationToken cancellationToken) =>
        context.Set<Sprint>()
            .Where(sprint => sprint.Id == id)
            .Select(sprint => sprint.Name)
            .FirstOrDefaultAsync(cancellationToken)!;

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
