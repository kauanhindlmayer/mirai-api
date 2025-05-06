namespace Application.Retrospectives.Queries.GetRetrospective;

public sealed class RetrospectiveResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int MaxVotesPerUser { get; init; }
    public IEnumerable<RetrospectiveColumnResponse> Columns { get; init; } = [];
}

public sealed class RetrospectiveColumnResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int Position { get; init; }
    public IEnumerable<RetrospectiveItemResponse> Items { get; init; } = [];
}

public sealed class RetrospectiveItemResponse
{
    public Guid Id { get; init; }
    public required string Content { get; init; }
    public int Position { get; init; }
    public Guid AuthorId { get; init; }
    public int Votes { get; init; }
    public DateTime CreatedAt { get; init; }
}
