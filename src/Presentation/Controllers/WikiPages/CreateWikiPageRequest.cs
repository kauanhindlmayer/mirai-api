namespace Presentation.Controllers.WikiPages;

/// <summary>
/// Request to create a new wiki page.
/// </summary>
/// <param name="Title">The title of the wiki page.</param>
/// <param name="Content">The content of the wiki page.</param>
/// <param name="ParentWikiPageId">The unique identifier of the parent wiki page.</param>
public sealed record CreateWikiPageRequest(
    string Title,
    string Content,
    Guid? ParentWikiPageId);