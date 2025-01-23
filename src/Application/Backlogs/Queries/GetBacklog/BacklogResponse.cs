namespace Application.Backlogs.Queries.GetBacklog;

public sealed class BacklogResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int? StoryPoints { get; init; }
    public IEnumerable<string> Tags { get; init; } = [];
    public IEnumerable<BacklogResponse> Children { get; init; } = [];
}