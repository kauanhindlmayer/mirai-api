namespace Contracts.WikiPages;

public sealed record UpdateWikiPageRequest
{
    /// <summary>
    /// The title of the wiki page.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The content of the wiki page.
    /// </summary>
    public required string Content { get; init; }
}