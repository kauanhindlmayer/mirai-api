using Mirai.Application.WikiPages.Commands.AddComment;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using Mirai.Application.WikiPages.Commands.DeleteWikiPage;
using Mirai.Application.WikiPages.Commands.UpdateWikiPage;
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

    public static UpdateWikiPageCommand CreateUpdateWikiPageCommand(
        Guid wikiPageId,
        string title = Constants.WikiPage.UpdatedTitle,
        string content = Constants.WikiPage.UpdatedContent)
    {
        return new(
            WikiPageId: wikiPageId,
            Title: title,
            Content: content);
    }

    public static DeleteWikiPageCommand CreateDeleteWikiPageCommand(Guid projectId)
    {
        return new(projectId);
    }

    public static AddCommentCommand CreateAddCommentCommand(
        Guid wikiPageId,
        string content = Constants.WikiPage.CommentContent)
    {
        return new(wikiPageId, content);
    }
}