namespace Contracts.Boards;

/// <summary>
/// Data transfer object for moving a card to a different column and position.
/// </summary>
/// <param name="TargetColumnId">The unique identifier of the target column.</param>
/// <param name="TargetPosition">The position of the card in the target column.</param>
public sealed record MoveCardRequest(
    Guid TargetColumnId,
    int TargetPosition);