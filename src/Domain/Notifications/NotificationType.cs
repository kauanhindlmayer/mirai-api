namespace Domain.Notifications;

public enum NotificationType
{
    AddedToProject,
    AddedToTeam,
    AddedToOrganization,
    AssignedWorkItemChanged,
    MentionedInWorkItemComment,
    MentionedInWikiPageComment,
    WorkItemCommentAdded,
}
