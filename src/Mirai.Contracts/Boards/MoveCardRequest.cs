namespace Mirai.Contracts.Boards;

public record MoveCardRequest(Guid TargetColumnId, int TargetPosition);