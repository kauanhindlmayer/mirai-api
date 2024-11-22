using Domain.Boards;

namespace Domain.UnitTests.Boards;

public static class BoardFactory
{
    public static Board CreateBoard(
        Guid? projectId = null,
        string name = "Board",
        string description = "Description")
    {
        return new(
            projectId ?? Guid.NewGuid(),
            name,
            description);
    }

    public static BoardColumn CreateBoardColumn(
        Guid? boardId = null,
        string name = "Column",
        int wipLimit = 5,
        string definitionOfDone = "Definition of Done")
    {
        return new(
            boardId ?? Guid.NewGuid(),
            name,
            wipLimit,
            definitionOfDone);
    }
}