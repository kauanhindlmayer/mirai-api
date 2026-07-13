namespace Presentation.Controllers.Notifications;

/// <summary>
/// Request to update the current user's notification preferences.
/// </summary>
/// <param name="MentionsEnabled">Whether to notify on @mentions in comments.</param>
/// <param name="AssignedWorkItemChangesEnabled">Whether to notify on changes to assigned work items.</param>
/// <param name="WorkItemCommentsEnabled">Whether to notify on new comments on assigned work items.</param>
/// <param name="MembershipEnabled">Whether to notify when added to a project, team, or organization.</param>
public sealed record UpdateNotificationPreferencesRequest(
    bool MentionsEnabled,
    bool AssignedWorkItemChangesEnabled,
    bool WorkItemCommentsEnabled,
    bool MembershipEnabled);
