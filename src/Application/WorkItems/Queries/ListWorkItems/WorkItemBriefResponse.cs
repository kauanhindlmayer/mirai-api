namespace Application.WorkItems.Queries.ListWorkItems;

public sealed class WorkItemBriefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public required string Type { get; init; }
    public AssigneeResponse? Assignee { get; init; }
    public IEnumerable<TagBriefResponse> Tags { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class TagBriefResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
}

public sealed class AssigneeResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}