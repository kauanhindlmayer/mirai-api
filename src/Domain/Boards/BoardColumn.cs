using Domain.Common;
using ErrorOr;

namespace Domain.Boards;

public sealed class BoardColumn : Entity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Position { get; private set; }
    public bool IsDefault => Position == 0;

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

        ShiftCards(0, 1);
        card.UpdatePosition(0);
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
        ShiftCards(card.Position, -1);
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

        ShiftCards(position, 1);
        card.UpdatePosition(position);
        Cards.Add(card);
        return Result.Success;
    }

    /// <summary>
    /// Shifts the positions of cards starting from a specific index
    /// by a given offset.
    /// </summary>
    /// <param name="fromIndex">The index from which to start shifting.</param>
    /// <param name="offset">
    /// The amount to shift the positions by. Positive values move cards
    /// down, negative values move them up.
    /// </param>
    private void ShiftCards(int fromIndex, int offset)
    {
        foreach (var card in Cards.Where(c => c.Position >= fromIndex))
        {
            card.UpdatePosition(card.Position + offset);
        }
    }
}