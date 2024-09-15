using ErrorOr;

namespace Mirai.Domain.Projects;

public static class ProjectErrors
{
    public static readonly Error WorkItemWithSameTitleAlreadyExists = Error.Conflict(
        code: "WorkItem.WorkItemWithSameTitleAlreadyExists",
        description: "A work item with the same title already exists.");
    public static readonly Error WikiPageWithSameTitleAlreadyExists = Error.Conflict(
        code: "WikiPage.WikiPageWithSameTitleAlreadyExists",
        description: "A wiki page with the same title already exists.");
}