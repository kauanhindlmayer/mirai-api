namespace Contracts.Boards;

public sealed record CreateBoardRequest(
    string Name,
    string Description);