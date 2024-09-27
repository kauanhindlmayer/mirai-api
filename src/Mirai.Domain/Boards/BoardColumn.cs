using Mirai.Domain.Common;

namespace Mirai.Domain.Boards;

public class BoardColumn : Entity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Position { get; private set; }
    public ICollection<BoardCard> Cards { get; private set; } = [];

    public BoardColumn(Guid boardId, string name, int position)
    {
        BoardId = boardId;
        Name = name;
        Position = position;
    }

    private BoardColumn()
    {
    }
}