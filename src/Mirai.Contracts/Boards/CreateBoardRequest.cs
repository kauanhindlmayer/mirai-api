namespace Mirai.Contracts.Boards;

public record CreateBoardRequest(
    Guid ProjectId,
    string Name,
    string Description);