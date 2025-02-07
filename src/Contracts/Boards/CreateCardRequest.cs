using Domain.WorkItems.Enums;

namespace Contracts.Boards;

public sealed record CreateCardRequest
{
    /// <summary>
    /// The type of the card.
    /// </summary>
    public WorkItemType Type { get; init; }

    /// <summary>
    /// The title of the card.
    /// </summary>
    public required string Title { get; init; }
}