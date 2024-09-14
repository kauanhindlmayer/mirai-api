namespace Mirai.Contracts.WorkItems;

public record WorkItemResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Status,
    string Type,
    List<CommentResponse> Comments,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CommentResponse(
    Guid Id,
    string Content,
    DateTime CreatedAt);