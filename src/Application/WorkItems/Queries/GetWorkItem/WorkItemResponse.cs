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
    public PlanningResponse? Planning { get; init; }
    public ClassificationResponse? Classification { get; init; }
    public Guid? AssigneeId { get; init; }
    public IEnumerable<WorkItemCommentResponse> Comments { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class PlanningResponse
{
    public int? StoryPoints { get; init; }
    public int? Priority { get; init; }
}

public sealed class ClassificationResponse
{
    public string? ValueArea { get; init; }
}

public sealed class WorkItemCommentResponse
{
    public Guid Id { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public required string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class AuthorResponse
{
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}

