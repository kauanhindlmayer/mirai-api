namespace Contracts.WikiPages;

public sealed record CreateWikiPageRequest(
    string Title,
    string Content,
    Guid? ParentWikiPageId);