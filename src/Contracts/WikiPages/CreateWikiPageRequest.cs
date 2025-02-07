namespace Contracts.WikiPages;

public sealed record CreateWikiPageRequest
{
    /// <summary>
    /// The title of the wiki page.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The content of the wiki page.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// The unique identifier of the parent wiki page.
    /// </summary>
    public Guid? ParentWikiPageId { get; init; }
}