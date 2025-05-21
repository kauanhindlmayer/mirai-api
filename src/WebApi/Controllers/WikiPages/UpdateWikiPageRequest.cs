namespace WebApi.Controllers.WikiPages;

/// <summary>
/// Data transfer object for updating a wiki page.
/// </summary>
/// <param name="Title">The title of the wiki page.</param>
/// <param name="Content">The content of the wiki page.</param>
public sealed record UpdateWikiPageRequest(string Title, string Content);