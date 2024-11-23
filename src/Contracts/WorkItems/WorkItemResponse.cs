using Contracts.Common;

namespace Contracts.WorkItems;

public sealed record WorkItemResponse(
    Guid Id,
    Guid ProjectId,
    int Code,
    string Title,
    string Description,
    string AcceptanceCriteria,
    WorkItemStatus Status,
    WorkItemType Type,
    List<CommentResponse> Comments,
    List<string> Tags,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CommentResponse(
    Guid Id,
    string Content,
    DateTime CreatedAt);