namespace Contracts.Boards;

public sealed record MoveCardRequest
{
    /// <summary>
    /// The unique identifier of the target column.
    /// </summary>
    public Guid TargetColumnId { get; init; }

    /// <summary>
    /// The position of the card in the target column.
    /// </summary>
    public int TargetPosition { get; init; }
}