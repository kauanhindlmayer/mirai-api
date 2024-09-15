using Mirai.Contracts.Common;

namespace Mirai.Contracts.WorkItems;

public record WorkItemResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    WorkItemStatus Status,
    WorkItemType Type,
    List<CommentResponse> Comments,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CommentResponse(
    Guid Id,
    string Content,
    DateTime CreatedAt);