using Mirai.Domain.Common;

namespace Mirai.Domain.Boards;

public class BoardColumn : Entity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Position { get; private set; }

    /// <summary>
    /// Gets the maximum number of cards that can be in this column at once.
    /// </summary>
    public int? WipLimit { get; private set; }
    public string DefinitionOfDone { get; private set; } = null!;
    public ICollection<BoardCard> Cards { get; private set; } = [];

    public BoardColumn(Guid boardId, string name, int position, int wipLimit, string definitionOfDone)
    {
        BoardId = boardId;
        Name = name;
        Position = position;
        WipLimit = wipLimit;
        DefinitionOfDone = definitionOfDone;
    }

    private BoardColumn()
    {
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }
}