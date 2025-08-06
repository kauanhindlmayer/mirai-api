using Domain.Backlogs;

namespace Presentation.Controllers.Backlogs;

/// <summary>
/// Request to get a backlog.
/// </summary>
/// <param name="SprintId">The sprint's unique identifier.</param>
/// <param name="BacklogLevel">The backlog level.</param>
public sealed record GetBacklogRequest(
    Guid? SprintId,
    BacklogLevel? BacklogLevel);