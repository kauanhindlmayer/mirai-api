namespace Contracts.WikiPages;

public sealed record WikiPageResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Content,
    AuthorResponse Author,
    List<WikiPageCommentResponse> Comments,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record WikiPageCommentResponse(
    Guid Id,
    AuthorResponse Author,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record AuthorResponse(
    string Name,
    string ImageUrl);