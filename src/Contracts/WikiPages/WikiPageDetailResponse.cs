namespace Contracts.WikiPages;

public sealed record WikiPageDetailResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Content,
    List<WikiPageCommentResponse> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record WikiPageCommentResponse(
    Guid Id,
    Guid UserId,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt);