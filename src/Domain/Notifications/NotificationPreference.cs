using Domain.Shared;
using Domain.Users;

namespace Domain.Notifications;

public sealed class NotificationPreference : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool MentionsEnabled { get; private set; } = true;
    public bool AssignedWorkItemChangesEnabled { get; private set; } = true;
    public bool WorkItemCommentsEnabled { get; private set; } = true;
    public bool MembershipEnabled { get; private set; } = true;

    public NotificationPreference(Guid userId)
    {
        UserId = userId;
    }

    private NotificationPreference()
    {
    }

    public void Update(
        bool mentionsEnabled,
        bool assignedWorkItemChangesEnabled,
        bool workItemCommentsEnabled,
        bool membershipEnabled)
    {
        MentionsEnabled = mentionsEnabled;
        AssignedWorkItemChangesEnabled = assignedWorkItemChangesEnabled;
        WorkItemCommentsEnabled = workItemCommentsEnabled;
        MembershipEnabled = membershipEnabled;
    }

    public bool IsEnabled(NotificationType type) => type switch
    {
        NotificationType.MentionedInWorkItemComment or NotificationType.MentionedInWikiPageComment
            => MentionsEnabled,
        NotificationType.AssignedWorkItemChanged => AssignedWorkItemChangesEnabled,
        NotificationType.WorkItemCommentAdded => WorkItemCommentsEnabled,
        NotificationType.AddedToProject or NotificationType.AddedToTeam or NotificationType.AddedToOrganization
            => MembershipEnabled,
        _ => true,
    };
}
