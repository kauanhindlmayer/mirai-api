namespace Application.Notifications.Queries.GetNotificationPreferences;

public sealed class NotificationPreferenceResponse
{
    public bool MentionsEnabled { get; init; }
    public bool AssignedWorkItemChangesEnabled { get; init; }
    public bool WorkItemCommentsEnabled { get; init; }
    public bool MembershipEnabled { get; init; }
}
