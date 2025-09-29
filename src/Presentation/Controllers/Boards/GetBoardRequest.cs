using Domain.Backlogs;

namespace Presentation.Controllers.Boards;

/// <summary>
/// Request to get a board.
/// </summary>
/// <param name="BacklogLevel">The backlog level to filter cards by.</param>
public sealed record GetBoardRequest(BacklogLevel? BacklogLevel);