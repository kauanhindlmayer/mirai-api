using Domain.WorkItems.Enums;

namespace Contracts.WorkItems;

public sealed record CreateWorkItemRequest
{
    /// <summary>
    /// The type of the work item.
    /// </summary>
    public WorkItemType Type { get; init; }

    /// <summary>
    /// The title of the work item.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The unique identifier of the team to assign the work item to.
    /// </summary>
    public Guid? AssignedTeamId { get; init; }
}