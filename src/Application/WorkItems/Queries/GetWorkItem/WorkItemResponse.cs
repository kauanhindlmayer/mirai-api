namespace Application.WorkItems.Queries.GetWorkItem;

public sealed class WorkItemResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? AcceptanceCriteria { get; init; }
    public required string Status { get; init; }
    public required string Type { get; init; }
    public PlanningResponse? Planning { get; init; }
    public ClassificationResponse? Classification { get; init; }
    public RelatedWorkItemResponse? ParentWorkItem { get; init; }
    public IEnumerable<RelatedWorkItemResponse> ChildWorkItems { get; init; } = [];
    public AssigneeResponse? Assignee { get; init; }
    public IEnumerable<WorkItemCommentResponse> Comments { get; init; } = [];
    public IEnumerable<TagResponse> Tags { get; init; } = [];
    public IEnumerable<WorkItemLinkResponse> Links { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class RelatedWorkItemResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public required string Type { get; init; }
    public AssigneeResponse? Assignee { get; init; }
}

public sealed class AssigneeResponse
{
    public Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public string? ImageUrl { get; init; }
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
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}

public sealed class TagResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
}

public sealed class WorkItemLinkResponse
{
    public Guid Id { get; init; }
    public RelatedWorkItemResponse TargetWorkItem { get; init; } = null!;
    public required string LinkType { get; init; }
    public string? Comment { get; init; }
}

