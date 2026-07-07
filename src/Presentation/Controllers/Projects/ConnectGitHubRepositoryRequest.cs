namespace Presentation.Controllers.Projects;

/// <summary>
/// Request to connect a project to a GitHub repository accessible to an
/// installed GitHub App installation.
/// </summary>
/// <param name="InstallationId">The GitHub App installation's unique identifier.</param>
/// <param name="RepositoryId">The repository's immutable GitHub numeric id.</param>
/// <param name="RepositoryOwner">The repository owner's login.</param>
/// <param name="RepositoryName">The repository name.</param>
public sealed record ConnectGitHubRepositoryRequest(
    long InstallationId,
    long RepositoryId,
    string RepositoryOwner,
    string RepositoryName);
