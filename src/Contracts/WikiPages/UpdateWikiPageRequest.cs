namespace Contracts.WikiPages;

public sealed record UpdateWikiPageRequest(
    string Title,
    string Content);