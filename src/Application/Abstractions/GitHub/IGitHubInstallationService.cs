namespace Application.Abstractions.GitHub;

public interface IGitHubInstallationService
{
    Task<IReadOnlyList<GitHubRepositorySummary>> ListRepositoriesAsync(
        long installationId,
        CancellationToken cancellationToken = default);
}

public sealed record GitHubRepositorySummary(long Id, string Owner, string Name);
