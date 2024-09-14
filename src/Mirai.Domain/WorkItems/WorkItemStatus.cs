using System.ComponentModel;

namespace Mirai.Domain.WorkItems;

public enum WorkItemStatus
{
    New,
    [Description("In Progress")]
    InProgress,
    Closed,
    Resolved,
    Reopened,
}