namespace Application.Boards.Queries.ListBoards;

public sealed class BoardBriefResponse
{
    public Guid Id { get; init; }
    public Guid TeamId { get; init; }
    public required string Name { get; init; }
}