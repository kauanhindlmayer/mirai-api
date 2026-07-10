using ErrorOr;

namespace Domain.Projects;

public static class ProjectErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Project.NotFound",
        description: "Project not found.");

    public static readonly Error AlreadyExists = Error.Validation(
        "Project.AlreadyExists",
        "A project with the same name already exists in the organization.");

    public static readonly Error WikiPageWithSameTitleAlreadyExists = Error.Conflict(
        code: "WikiPage.WikiPageWithSameTitleAlreadyExists",
        description: "A wiki page with the same title already exists.");

    public static readonly Error TeamWithSameNameAlreadyExists = Error.Conflict(
        code: "Team.TeamWithSameNameAlreadyExists",
        description: "A team with the same name already exists.");

    public static readonly Error UserNotInOrganization = Error.Validation(
        "Project.UserNotInOrganization",
        "User must be a member of the organization to be added to the project.");

    public static readonly Error UserAlreadyExists = Error.Conflict(
        "Project.UserAlreadyExists",
        "User already exists in this project.");

    public static readonly Error UserHasAssignedWorkItems = Error.Conflict(
        "Project.UserHasAssignedWorkItems",
        "Cannot remove user who has assigned work items.");

    public static readonly Error UserIsInTeams = Error.Conflict(
        "Project.UserIsInTeams",
        "Cannot remove user who is a member of project teams.");

    public static readonly Error CannotRemoveLastAdmin = Error.Conflict(
        "Project.CannotRemoveLastAdmin",
        "Cannot remove or demote the last remaining Admin of this project.");

    public static readonly Error GitHubRepositoryAlreadyConnected = Error.Conflict(
        "Project.GitHubRepositoryAlreadyConnected",
        "This project is already connected to a GitHub repository.");

    public static readonly Error NoGitHubRepositoryConnected = Error.Validation(
        "Project.NoGitHubRepositoryConnected",
        "This project has no connected GitHub repository.");

    public static readonly Error GitHubRepositoryAlreadyConnectedElsewhere = Error.Conflict(
        "Project.GitHubRepositoryAlreadyConnectedElsewhere",
        "This GitHub repository is already connected to another project.");

    public static readonly Error InvalidGitHubInstallationState = Error.Validation(
        "Project.InvalidGitHubInstallationState",
        "This GitHub installation request is invalid or has expired. Please try connecting again.");
}