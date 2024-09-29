namespace Contracts.Boards;

public record MoveCardRequest(Guid TargetColumnId, int TargetPosition);