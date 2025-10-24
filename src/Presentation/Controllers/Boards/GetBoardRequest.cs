using Domain.Backlogs;

namespace Presentation.Controllers.Boards;

/// <summary>
/// Request to get a board.
/// </summary>
/// <param name="BacklogLevel">The backlog level to filter cards by.</param>
/// <param name="PageSize">The number of cards to return per column.</param>
public sealed record GetBoardRequest(BacklogLevel? BacklogLevel, int PageSize = 20);