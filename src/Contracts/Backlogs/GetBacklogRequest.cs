using Domain.Backlogs;

namespace Contracts.Backlogs;

/// <summary>
/// Data transfer object for retrieving a backlog.
/// </summary>
/// <param name="SprintId">The sprint's unique identifier.</param>
/// <param name="BacklogLevel">The backlog level.</param>
public sealed record GetBacklogRequest(
    Guid? SprintId,
    BacklogLevel? BacklogLevel);