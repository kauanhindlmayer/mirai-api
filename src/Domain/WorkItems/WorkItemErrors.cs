using ErrorOr;

namespace Domain.WorkItems;

public static class WorkItemErrors
{
    public static readonly Error WorkItemNotFound = Error.NotFound(
        code: "WorkItem.WorkItemNotFound",
        description: "Work Item not found.");

    public static readonly Error WorkItemCommentNotFound = Error.NotFound(
        code: "WorkItem.WorkItemCommentNotFound",
        description: "Work Item Comment not found.");

    public static readonly Error AssigneeNotFound = Error.NotFound(
        code: "WorkItem.AssigneeNotFound",
        description: "Assignee not found.");

    public static readonly Error CannotAddCommentToClosedWorkItem = Error.Validation(
        code: "WorkItem.CannotAddCommentToClosedWorkItem",
        description: "Cannot add comment to a closed work item");

    public static readonly Error WorkItemTagAlreadyExists = Error.Conflict(
        code: "WorkItem.WorkItemTagAlreadyExists",
        description: "Work Item Tag already exists.");

    public static readonly Error WorkItemTagNotFound = Error.NotFound(
        code: "WorkItem.WorkItemTagNotFound",
        description: "Work Item Tag not found.");
}
