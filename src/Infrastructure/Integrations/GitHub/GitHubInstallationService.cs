using Application.Abstractions.GitHub;

namespace Infrastructure.Integrations.GitHub;

internal sealed class GitHubInstallationService : IGitHubInstallationService
{
    private readonly IGitHubAppClientFactory _clientFactory;

    public GitHubInstallationService(IGitHubAppClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<GitHubRepositorySummary>> ListRepositoriesAsync(
        long installationId,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientFactory.CreateInstallationClientAsync(installationId, cancellationToken);
        var response = await client.GitHubApps.Installation.GetAllRepositoriesForCurrent();

        return response.Repositories
            .Select(repository => new GitHubRepositorySummary(
                repository.Id,
                repository.Owner.Login,
                repository.Name))
            .ToList();
    }
}
