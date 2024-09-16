using Mirai.Application.Organizations.Commands.CreateOrganization;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using TestCommon.TestConstants;

namespace TestCommon.WikiPages;

public static class WikiPageCommandFactory
{
    public static CreateWikiPageCommand CreateCreateWikiPageCommand(
        string title = Constants.WikiPage.Title,
        string content = Constants.WikiPage.Content,
        Guid? projectId = null,
        Guid? parentWikiPageId = null)
    {
        return new(
            ProjectId: projectId ?? Constants.WikiPage.ProjectId,
            Title: title,
            Content: content,
            ParentWikiPageId: parentWikiPageId);
    }
}