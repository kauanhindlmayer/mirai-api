using ErrorOr;

namespace Mirai.Domain.WikiPages;

public static class WikiPageErrors
{
    public static readonly Error ParentWikiPageNotFound = Error.NotFound(
        code: "WikiPage.ParentWikiPageNotFound",
        description: "Parent Wiki Page not found.");

    public static readonly Error WikiPageNotFound = Error.NotFound(
        code: "WikiPage.WikiPageNotFound",
        description: "Wiki Page not found.");

    public static readonly Error WikiPageHasSubWikiPages = Error.Conflict(
        code: "WikiPage.WikiPageHasSubWikiPages",
        description: "Wiki Page has sub Wiki Pages.");
}