using ErrorOr;

namespace Mirai.Domain.Projects;

public static class ProjectErrors
{
    public static readonly Error ProjectNotFound = Error.NotFound(
        code: "Project.ProjectNotFound",
        description: "Project not found.");

    public static readonly Error ProjectWithSameNameAlreadyExists = Error.Validation(
        "Project.ProjectWithSameNameAlreadyExists",
        "A project with the same name already exists in the organization.");

    public static readonly Error WorkItemWithSameTitleAlreadyExists = Error.Conflict(
        code: "WorkItem.WorkItemWithSameTitleAlreadyExists",
        description: "A work item with the same title already exists.");

    public static readonly Error WikiPageWithSameTitleAlreadyExists = Error.Conflict(
        code: "WikiPage.WikiPageWithSameTitleAlreadyExists",
        description: "A wiki page with the same title already exists.");
}