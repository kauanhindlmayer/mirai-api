using Domain.Boards;
using TestCommon.TestConstants;

namespace TestCommon.Boards;

public static class BoardFactory
{
    public static Board CreateBoard(
        string name = Constants.Board.Name,
        string description = Constants.Board.Description,
        Guid? projectId = null)
    {
        return new(
            projectId ?? Constants.Project.Id,
            name,
            description);
    }

    public static BoardColumn CreateBoardColumn(
        string name = Constants.Board.ColumnName,
        int position = 0,
        int wipLimit = Constants.Board.WipLimit,
        string definitionOfDone = Constants.Board.DefinitionOfDone,
        Guid? boardId = null)
    {
        return new(
            boardId ?? Constants.Board.Id,
            name,
            wipLimit,
            definitionOfDone);
    }
}