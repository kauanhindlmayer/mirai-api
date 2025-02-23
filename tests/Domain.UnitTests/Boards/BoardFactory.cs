using Domain.Boards;

namespace Domain.UnitTests.Boards;

public static class BoardFactory
{
    public const string Name = "Test Board";

    public static Board CreateBoard(
        Guid? projectId = null,
        string name = Name)
    {
        return new(
            projectId ?? Guid.NewGuid(),
            name);
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

    public static BoardCard CreateBoardCard(
        Guid? columnId = null,
        Guid? workItemId = null,
        int position = 0)
    {
        return new(
            columnId ?? Guid.NewGuid(),
            workItemId ?? Guid.NewGuid(),
            position);
    }
}