using Domain.WorkItems.Enums;

namespace Presentation.Controllers.Boards;

/// <summary>
/// Request to create a new card on a board.
/// </summary>
/// <param name="Type">The type of the card.</param>
/// <param name="Title">The title of the card.</param>
public sealed record CreateCardRequest(
    WorkItemType Type,
    string Title);