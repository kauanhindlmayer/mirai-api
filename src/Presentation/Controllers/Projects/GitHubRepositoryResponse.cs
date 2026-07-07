namespace Presentation.Controllers.Projects;

/// <summary>
/// A repository accessible to a GitHub App installation.
/// </summary>
/// <param name="Id">The repository's immutable GitHub numeric id.</param>
/// <param name="Owner">The repository owner's login.</param>
/// <param name="Name">The repository name.</param>
public sealed record GitHubRepositoryResponse(long Id, string Owner, string Name);
