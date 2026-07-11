using Domain.Shared;
using Domain.Users;

namespace Domain.WorkItems;

public sealed class WorkItemChangeSet : Entity
{
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public Guid? ChangedByUserId { get; private set; }
    public User? ChangedByUser { get; private set; }
    public string? SystemActor { get; private set; }
    public ICollection<WorkItemChange> Changes { get; private set; } = [];

    public WorkItemChangeSet(Guid workItemId, Guid? changedByUserId, string? systemActor = null)
    {
        WorkItemId = workItemId;
        ChangedByUserId = changedByUserId;
        SystemActor = systemActor;
    }

    private WorkItemChangeSet()
    {
    }

    public void AddChange(string fieldName, string? oldValue, string? newValue)
    {
        Changes.Add(new WorkItemChange(fieldName, oldValue, newValue));
    }
}
