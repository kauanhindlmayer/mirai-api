namespace Application.WorkItems.Queries.Common;

public sealed class WorkItemBriefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public required string Type { get; init; }
    public IEnumerable<TagBriefResponse> Tags { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class TagBriefResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
}