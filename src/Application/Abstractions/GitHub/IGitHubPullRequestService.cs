namespace Application.Abstractions.GitHub;

public interface IGitHubPullRequestService
{
    Task<IReadOnlyList<GitHubPullRequestSummary>> SearchPullRequestsAsync(
        long installationId,
        string repositoryOwner,
        string repositoryName,
        string query,
        CancellationToken cancellationToken = default);

    Task<GitHubPullRequestSummary?> GetPullRequestAsync(
        long installationId,
        string repositoryOwner,
        string repositoryName,
        int pullRequestNumber,
        CancellationToken cancellationToken = default);
}

public sealed record GitHubPullRequestSummary(
    long Id,
    int Number,
    string Title,
    string HtmlUrl,
    bool IsOpen,
    bool IsMerged,
    string? AuthorLogin);
