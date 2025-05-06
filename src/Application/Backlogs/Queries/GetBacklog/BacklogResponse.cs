namespace Application.Backlogs.Queries.GetBacklog;

public sealed class BacklogResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public int? StoryPoints { get; init; }
    public required string ValueArea { get; init; }
    public IEnumerable<string> Tags { get; init; } = [];
    public IEnumerable<BacklogResponse> Children { get; init; } = [];
}