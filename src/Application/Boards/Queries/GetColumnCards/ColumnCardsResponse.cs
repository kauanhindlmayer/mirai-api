using Application.Boards.Queries.GetBoard;

namespace Application.Boards.Queries.GetColumnCards;

public sealed class ColumnCardsResponse
{
    public IEnumerable<BoardCardResponse> Cards { get; init; } = [];
    public bool HasMoreCards { get; init; }
    public int TotalCardCount { get; init; }
}
