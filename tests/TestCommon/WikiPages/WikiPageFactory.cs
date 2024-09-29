using Domain.WikiPages;
using TestCommon.TestConstants;

namespace TestCommon.WikiPages;

public static class WikiPageFactory
{
    public static WikiPage CreateWikiPage(
        Guid? projectId = null,
        string title = Constants.WikiPage.Title,
        string content = Constants.WikiPage.Content,
        Guid? parentWikiPageId = null)
    {
        return new(
            projectId: projectId ?? Constants.WikiPage.ProjectId,
            title: title,
            content: content,
            parentWikiPageId: parentWikiPageId);
    }
}