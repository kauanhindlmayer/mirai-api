using ErrorOr;

namespace Domain.WorkItems;

public static class WorkItemErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "WorkItem.NotFound",
        description: "Work Item not found.");

    public static readonly Error CommentNotFound = Error.NotFound(
        code: "WorkItem.CommentNotFound",
        description: "Work Item Comment not found.");

    public static readonly Error AssigneeNotFound = Error.NotFound(
        code: "WorkItem.AssigneeNotFound",
        description: "Assignee not found.");

    public static readonly Error CannotAddCommentToClosedWorkItem = Error.Validation(
        code: "WorkItem.CannotAddCommentToClosedWorkItem",
        description: "Cannot add comment to a closed work item");
}
