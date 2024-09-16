using ErrorOr;

namespace Mirai.Domain.WikiPages;

public static class WikiPageErrors
{
    public static readonly Error ParentWikiPageNotFound = Error.NotFound(
        code: "WikiPage.ParentWikiPageNotFound",
        description: "Parent Wiki Page not found.");
}