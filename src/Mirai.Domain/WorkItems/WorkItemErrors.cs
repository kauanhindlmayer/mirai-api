using ErrorOr;

namespace Mirai.Domain.WorkItems;

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
}
