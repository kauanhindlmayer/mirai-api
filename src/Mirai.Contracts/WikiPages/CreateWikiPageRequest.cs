namespace Mirai.Contracts.WikiPages;

public record CreateWikiPageRequest(
    string Title,
    string Content,
    Guid? ParentWikiPageId);