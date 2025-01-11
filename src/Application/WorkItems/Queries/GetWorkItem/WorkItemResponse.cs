namespace Application.WorkItems.Queries.GetWorkItem;

public sealed class WorkItemResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public int Code { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AcceptanceCriteria { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public IEnumerable<CommentResponse> Comments { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class CommentResponse
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
