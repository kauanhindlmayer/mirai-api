using Domain.Notifications;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
/// Creates <see cref="Notification"/> rows from what other interceptors/the tracked
/// entity graph produced during this save. Registered immediately after
/// <see cref="WorkItemChangeHistoryInterceptor"/> so it can read the <see cref="WorkItemChangeSet"/>
/// that interceptor just staged - reusing its already-resolved field names rather than
/// re-diffing the work item - and before <see cref="DomainEventPublishingInterceptor"/>, so a
/// freshly staged <see cref="Notification"/>'s own domain event is included in that interceptor's
/// single publish pass rather than missed.
/// </summary>
internal sealed class NotificationInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await CreateAssignedWorkItemChangedNotificationsAsync(eventData.Context, cancellationToken);
            await CreateWorkItemCommentNotificationsAsync(eventData.Context, cancellationToken);
            await CreateWikiPageCommentNotificationsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static async Task CreateAssignedWorkItemChangedNotificationsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var changeSets = context.ChangeTracker.Entries<WorkItemChangeSet>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var changeSet in changeSets)
        {
            if (changeSet.ChangedByUserId is null)
            {
                continue;
            }

            // Read the tracked entity rather than querying the database: the database
            // still holds pre-save values at this point, so a reassignment in the same
            // save would otherwise resolve to the outgoing assignee, not the new one.
            var workItem = FindTrackedWorkItem(context, changeSet.WorkItemId);

            if (workItem?.AssigneeId is null || workItem.AssigneeId == changeSet.ChangedByUserId)
            {
                continue;
            }

            var actorName = await context.Set<User>()
                .Where(u => u.Id == changeSet.ChangedByUserId)
                .Select(u => u.FirstName + " " + u.LastName)
                .FirstOrDefaultAsync(cancellationToken);

            var fieldNames = string.Join(", ", changeSet.Changes.Select(c => c.FieldName).Distinct());
            var message = $"{actorName} changed {fieldNames} on \"{workItem.Title}\".";

            await TryAddNotificationAsync(
                context,
                workItem.AssigneeId.Value,
                NotificationType.AssignedWorkItemChanged,
                changeSet.WorkItemId,
                message,
                cancellationToken);
        }
    }

    private static async Task CreateWorkItemCommentNotificationsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var comments = context.ChangeTracker.Entries<WorkItemComment>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var comment in comments)
        {
            foreach (var mentionedUserId in MentionParser.ParseMentionedUserIds(comment.Content))
            {
                if (mentionedUserId == comment.AuthorId)
                {
                    continue;
                }

                await TryAddNotificationAsync(
                    context,
                    mentionedUserId,
                    NotificationType.MentionedInWorkItemComment,
                    comment.WorkItemId,
                    "You were mentioned in a comment on a work item.",
                    cancellationToken);
            }

            var workItem = FindTrackedWorkItem(context, comment.WorkItemId);

            if (workItem?.AssigneeId is null || workItem.AssigneeId == comment.AuthorId)
            {
                continue;
            }

            await TryAddNotificationAsync(
                context,
                workItem.AssigneeId.Value,
                NotificationType.WorkItemCommentAdded,
                comment.WorkItemId,
                $"New comment on \"{workItem.Title}\".",
                cancellationToken);
        }
    }

    private static async Task CreateWikiPageCommentNotificationsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var comments = context.ChangeTracker.Entries<WikiPageComment>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var comment in comments)
        {
            foreach (var mentionedUserId in MentionParser.ParseMentionedUserIds(comment.Content))
            {
                if (mentionedUserId == comment.AuthorId)
                {
                    continue;
                }

                await TryAddNotificationAsync(
                    context,
                    mentionedUserId,
                    NotificationType.MentionedInWikiPageComment,
                    comment.WikiPageId,
                    "You were mentioned in a comment on a wiki page.",
                    cancellationToken);
            }
        }
    }

    private static async Task TryAddNotificationAsync(
        DbContext context,
        Guid recipientUserId,
        NotificationType type,
        Guid entityId,
        string message,
        CancellationToken cancellationToken)
    {
        var preference = await context.Set<NotificationPreference>()
            .FirstOrDefaultAsync(p => p.UserId == recipientUserId, cancellationToken);

        if (!(preference?.IsEnabled(type) ?? true))
        {
            return;
        }

        context.Set<Notification>().Add(new Notification(recipientUserId, type, entityId, message));
    }

    private static WorkItem? FindTrackedWorkItem(DbContext context, Guid workItemId)
    {
        return context.ChangeTracker.Entries<WorkItem>()
            .Select(entry => entry.Entity)
            .FirstOrDefault(wi => wi.Id == workItemId);
    }
}
