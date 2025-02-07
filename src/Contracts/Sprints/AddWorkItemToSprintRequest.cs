namespace Contracts.Sprints;

public sealed record AddWorkItemToSprintRequest
{
    /// <summary>
    /// The work item's unique identifier.
    /// </summary>
    public Guid WorkItemId { get; init; }
}