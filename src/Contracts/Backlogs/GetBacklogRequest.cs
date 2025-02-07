using Domain.Backlogs;

namespace Contracts.Backlogs;

public sealed record GetBacklogRequest
{
    /// <summary>
    /// The sprint's unique identifier.
    /// </summary>
    public Guid? SprintId { get; init; }

    /// <summary>
    /// The backlog level.
    /// </summary>
    public BacklogLevel? BacklogLevel { get; init; }
}