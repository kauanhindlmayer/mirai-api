using ErrorOr;

namespace Mirai.Domain.Boards;

public static class BoardErrors
{
    public static readonly Error BoardNotFound = Error.NotFound(
        "Board.BoardNotFound",
        "Board not found.");
}