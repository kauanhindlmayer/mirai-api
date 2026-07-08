namespace Presentation.Controllers.Projects;

/// <summary>
/// Response containing the GitHub App installation URL to redirect to.
/// </summary>
/// <param name="Url">The GitHub App installation URL.</param>
public sealed record GitHubInstallUrlResponse(string Url);
