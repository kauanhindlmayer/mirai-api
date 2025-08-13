using Domain.Boards;

namespace Domain.UnitTests.Boards;

public static class BoardFactory
{
    public const string Name = "Test Board";
    public static readonly Guid TeamId = Guid.NewGuid();

    public static Board Create(
        Guid? teamId = null,
        string? name = null)
    {
        return new(
            teamId ?? TeamId,
            name ?? Name);
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