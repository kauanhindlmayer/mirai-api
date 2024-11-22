using Domain.WikiPages;

namespace Domain.UnitTests.WikiPages;

public static class WikiPageFactory
{
    public static WikiPage CreateWikiPage(
        Guid? projectId = null,
        string title = "Title",
        string content = "Content",
        Guid? parentWikiPageId = null)
    {
        return new(
            projectId ?? Guid.NewGuid(),
            title,
            content,
            parentWikiPageId);
    }
}