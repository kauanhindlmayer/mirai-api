namespace Application.WorkItems.Queries.Common;

public sealed class WorkItemBriefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public IEnumerable<TagBriefResponse> Tags { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class TagBriefResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}