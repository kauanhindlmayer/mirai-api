namespace Presentation.Controllers.WikiPages;

/// <summary>
/// Request to update a wiki page.
/// </summary>
/// <param name="Title">The title of the wiki page.</param>
/// <param name="Content">The content of the wiki page.</param>
public sealed record UpdateWikiPageRequest(string Title, string Content);