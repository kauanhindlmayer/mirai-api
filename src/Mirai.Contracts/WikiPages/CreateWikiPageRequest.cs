namespace Mirai.Contracts.WikiPages;

public record CreateWikiPageRequest(
    Guid ProjectId,
    string Title,
    string Content);