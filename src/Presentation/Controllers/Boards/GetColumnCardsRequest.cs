using Domain.Backlogs;

namespace Presentation.Controllers.Boards;

/// <summary>
/// Request to get cards for a specific column.
/// </summary>
/// <param name="BacklogLevel">The backlog level to filter cards by.</param>
/// <param name="PageSize">The number of cards to return.</param>
/// <param name="Page">The page number (1-based).</param>
public sealed record GetColumnCardsRequest(
    BacklogLevel? BacklogLevel,
    int PageSize = 20,
    int Page = 1);
