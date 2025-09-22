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

    public static readonly Error CommentNotOwned = Error.Unauthorized(
        code: "WikiPage.CommentNotOwned",
        description: "You are not authorized to update this comment.");

    public static readonly Error InvalidPosition = Error.Validation(
        code: "WikiPage.InvalidPosition",
        description: "Position must be greater than or equal to 0 and less than or equal to the number of sub wiki pages.");
}