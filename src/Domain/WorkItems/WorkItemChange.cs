using Domain.Shared;

namespace Domain.WorkItems;

public sealed class WorkItemChange : Entity
{
    public Guid WorkItemChangeSetId { get; private set; }
    public WorkItemChangeSet WorkItemChangeSet { get; private set; } = null!;
    public string FieldName { get; private set; } = null!;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    internal WorkItemChange(string fieldName, string? oldValue, string? newValue)
    {
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
    }

    private WorkItemChange()
    {
    }
}
