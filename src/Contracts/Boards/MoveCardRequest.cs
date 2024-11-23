namespace Contracts.Boards;

public sealed record MoveCardRequest(
    Guid TargetColumnId,
    int TargetPosition);