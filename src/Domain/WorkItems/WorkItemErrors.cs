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

    public static readonly Error CommentNotOwned = Error.Unauthorized(
        code: "WorkItem.CommentNotOwned",
        description: "You are not authorized to update this comment.");

    public static readonly Error AssigneeNotFound = Error.NotFound(
        code: "WorkItem.AssigneeNotFound",
        description: "Assignee not found.");

    public static readonly Error CannotAddCommentToClosedWorkItem = Error.Validation(
        code: "WorkItem.CannotAddCommentToClosedWorkItem",
        description: "Cannot add comment to a closed work item");

    public static readonly Error CannotLinkToSelf = Error.Validation(
        code: "WorkItem.CannotLinkToSelf",
        description: "A work item cannot be linked to itself");

    public static readonly Error LinkAlreadyExists = Error.Conflict(
        code: "WorkItem.LinkAlreadyExists",
        description: "This link already exists");

    public static readonly Error LinkNotFound = Error.NotFound(
        code: "WorkItem.LinkNotFound",
        description: "The specified link was not found");

    public static readonly Error CircularDependency = Error.Validation(
        code: "WorkItem.CircularDependency",
        description: "This link would create a circular dependency");

    public static readonly Error TargetWorkItemNotFound = Error.NotFound(
        code: "WorkItem.TargetWorkItemNotFound",
        description: "Target work item not found");
}
