using ErrorOr;

namespace Domain.WikiPages;

public static class WikiPageErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "WikiPage.NotFound",
        description: "Wiki Page not found.");

    public static readonly Error ParentWikiPageNotFound = Error.NotFound(
        code: "WikiPage.ParentWikiPageNotFound",
        description: "Parent Wiki Page not found.");

    public static readonly Error HasSubWikiPages = Error.Conflict(
        code: "WikiPage.HasSubWikiPages",
        description: "Wiki Page has sub Wiki Pages.");

    public static readonly Error CommentNotFound = Error.NotFound(
        code: "WikiPage.CommentNotFound",
        description: "Comment not found.");
}