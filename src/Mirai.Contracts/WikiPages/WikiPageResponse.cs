namespace Mirai.Contracts.WikiPages;

public record WikiPageResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);