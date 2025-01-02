namespace Application.Boards.Queries.ListBoards;

public sealed class BoardSummaryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
}