using Domain.WikiPages;

namespace Domain.UnitTests.WikiPages;

internal static class WikiPageFactory
{
    public const string Title = "Title";
    public const string Content = "Content";
    public static readonly Guid ProjectId = Guid.NewGuid();
    public static readonly Guid AuthorId = Guid.NewGuid();
    public static readonly Guid ParentWikiPageId = Guid.NewGuid();

    public static WikiPage Create(
        Guid? projectId = null,
        string? title = null,
        string? content = null,
        Guid? authorId = null,
        Guid? parentWikiPageId = null)
    {
        return new(
            projectId ?? ProjectId,
            title ?? Title,
            content ?? Content,
            authorId ?? AuthorId,
            parentWikiPageId ?? ParentWikiPageId);
    }
}