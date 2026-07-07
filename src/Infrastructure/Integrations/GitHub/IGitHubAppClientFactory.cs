using Octokit;

namespace Infrastructure.Integrations.GitHub;

/// <summary>
/// Creates Octokit clients authenticated as a specific GitHub App
/// installation. Internal to the Infrastructure layer: the rest of the
/// codebase reaches GitHub only through <see cref="Application.Abstractions.GitHub.IGitHubInstallationService"/>
/// and <see cref="Application.Abstractions.GitHub.IGitHubPullRequestService"/>.
/// </summary>
internal interface IGitHubAppClientFactory
{
    Task<IGitHubClient> CreateInstallationClientAsync(
        long installationId,
        CancellationToken cancellationToken = default);
}
