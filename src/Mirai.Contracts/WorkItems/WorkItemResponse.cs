using Mirai.Contracts.Common;

namespace Mirai.Contracts.WorkItems;

public record WorkItemResponse(
    Guid Id,
    Guid ProjectId,
    int Code,
    string Title,
    string? Description,
    WorkItemStatus Status,
    WorkItemType Type,
    List<CommentResponse> Comments,
    List<TagResponse> Tags,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CommentResponse(
    Guid Id,
    string Content,
    DateTime CreatedAt);

public record TagResponse(
    Guid Id,
    string Name);