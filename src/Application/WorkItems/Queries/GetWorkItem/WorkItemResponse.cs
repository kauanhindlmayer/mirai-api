namespace Application.WorkItems.Queries.GetWorkItem;

public sealed class WorkItemResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string AcceptanceCriteria { get; init; }
    public required string Status { get; init; }
    public required string Type { get; init; }
    public IEnumerable<CommentResponse> Comments { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class CommentResponse
{
    public Guid Id { get; init; }
    public required string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
