namespace Contracts.WorkItems;

public sealed record AssignWorkItemRequest
{
    /// <summary>
    /// The unique identifier of the user to assign the work item to.
    /// </summary>
    public Guid AssigneeId { get; init; }
}