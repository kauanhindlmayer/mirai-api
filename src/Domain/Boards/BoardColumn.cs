using Domain.Common;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Boards;

public sealed class BoardColumn : Entity
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
    public string? DefinitionOfDone { get; private set; }
    public List<BoardCard> Cards { get; private set; } = [];

    public BoardColumn(
        Guid boardId,
        string name,
        int? wipLimit = null,
        string? definitionOfDone = null)
    {
        BoardId = boardId;
        Name = name;
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

    public ErrorOr<BoardCard> AddCard(BoardCard card)
    {
        if (Cards.Any(c => c.WorkItemId == card.WorkItemId))
        {
            return BoardErrors.CardAlreadyExists;
        }

        card.UpdatePosition(Cards.Count);
        Cards.Add(card);
        return card;
    }

    public ErrorOr<BoardCard> RemoveCard(Guid cardId)
    {
        var card = Cards.FirstOrDefault(c => c.Id == cardId);
        if (card is null)
        {
            return BoardErrors.CardNotFound;
        }

        Cards.Remove(card);
        ReorderCards();
        return card;
    }

    public ErrorOr<Success> AddCardAtPosition(BoardCard card, int position)
    {
        if (position < 0 || position > Cards.Count)
        {
            return BoardErrors.InvalidPosition;
        }

        if (Cards.Any(c => c.WorkItemId == card.WorkItemId))
        {
            return BoardErrors.CardAlreadyExists;
        }

        Cards.Insert(position, card);
        ReorderCards();
        return Result.Success;
    }

    public void ReorderCards()
    {
        var position = 0;
        foreach (var card in Cards)
        {
            card.UpdatePosition(position);
            position++;
        }
    }
}