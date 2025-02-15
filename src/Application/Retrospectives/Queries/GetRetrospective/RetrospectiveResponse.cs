namespace Application.Retrospectives.Queries.GetRetrospective;

public sealed class RetrospectiveResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int MaxVotesPerUser { get; init; }
    public IEnumerable<RetrospectiveColumnResponse> Columns { get; init; } = [];
}

public sealed class RetrospectiveColumnResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Position { get; init; }
    public IEnumerable<RetrospectiveItemResponse> Items { get; init; } = [];
}

public sealed class RetrospectiveItemResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Position { get; init; }
    public Guid AuthorId { get; init; }
    public int Votes { get; init; }
}
