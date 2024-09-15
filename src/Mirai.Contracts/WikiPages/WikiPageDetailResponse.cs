namespace Mirai.Contracts.WikiPages;

public record WikiPageDetailResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record WikiPageSummaryResponse(
    Guid Id,
    Guid ProjectId,
    string Title);