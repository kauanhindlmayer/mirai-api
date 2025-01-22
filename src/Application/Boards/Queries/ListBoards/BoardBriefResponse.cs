namespace Application.Boards.Queries.ListBoards;

public sealed class BoardBriefResponse
{
    public Guid Id { get; init; }
    public Guid TeamId { get; init; }
    public string Name { get; init; } = null!;
}