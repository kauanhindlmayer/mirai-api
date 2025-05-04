using Domain.WorkItems.Enums;

namespace Contracts.Boards;

/// <summary>
/// Data transfer object for creating a card in a column.
/// </summary>
/// <param name="Type">The type of the card.</param>
/// <param name="Title">The title of the card.</param>
public sealed record CreateCardRequest(
    WorkItemType Type,
    string Title);