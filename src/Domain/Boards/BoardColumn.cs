using Domain.Common;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Boards;

public class BoardColumn : Entity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Position { get; private set; }

    /// <summary>
    /// Gets the Work-In-Progress limit, the recommended max cards for this column.
    /// This is advisory only, not enforced.
    /// </summary>
    public int? WipLimit { get; private set; }
    public string DefinitionOfDone { get; private set; } = null!;
    public List<BoardCard> Cards { get; private set; } = [];

    public BoardColumn(Guid boardId, string name, int position, int? wipLimit, string definitionOfDone)
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

    public ErrorOr<BoardCard> AddCard(WorkItem workItem)
    {
        if (Cards.Any(c => c.WorkItem.Id == workItem.Id))
        {
            return BoardErrors.CardAlreadyExists;
        }

        var position = Cards.Count;
        var card = new BoardCard(Id, workItem.Id, position);
        Cards.Add(card);
        return card;
    }

    public void ReorderCards()
    {
        var position = 0;
        foreach (var card in Cards.OrderBy(c => c.Position))
        {
            card.UpdatePosition(position);
            position++;
        }
    }
}