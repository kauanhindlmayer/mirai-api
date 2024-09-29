using Ardalis.SmartEnum;

namespace Domain.WorkItems.Enums;

public class WorkItemStatus(string name, int value)
    : SmartEnum<WorkItemStatus>(name, value)
{
    public static readonly WorkItemStatus New = new(nameof(New), 0);
    public static readonly WorkItemStatus InProgress = new(nameof(InProgress), 1);
    public static readonly WorkItemStatus Closed = new(nameof(Closed), 2);
    public static readonly WorkItemStatus Resolved = new(nameof(Resolved), 3);
    public static readonly WorkItemStatus Reopened = new(nameof(Reopened), 4);
    public static readonly WorkItemStatus Removed = new(nameof(Removed), 5);
}