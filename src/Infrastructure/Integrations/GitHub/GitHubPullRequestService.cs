using Application.Abstractions.GitHub;
using Octokit;

namespace Infrastructure.Integrations.GitHub;

internal sealed class GitHubPullRequestService : IGitHubPullRequestService
{
    private readonly IGitHubAppClientFactory _clientFactory;

    public GitHubPullRequestService(IGitHubAppClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<GitHubPullRequestSummary>> SearchPullRequestsAsync(
        long installationId,
        string repositoryOwner,
        string repositoryName,
        string query,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientFactory.CreateInstallationClientAsync(installationId, cancellationToken);

        var request = new SearchIssuesRequest(query)
        {
            Type = IssueTypeQualifier.PullRequest,
        };
        request.Repos.Add(repositoryOwner, repositoryName);

        var result = await client.Search.SearchIssues(request);

        return result.Items
            .Select(issue => new GitHubPullRequestSummary(
                issue.Id,
                issue.Number,
                issue.Title,
                issue.HtmlUrl,
                IsOpen: issue.State.Value == ItemState.Open,
                IsMerged: issue.PullRequest?.Merged ?? false,
                issue.User?.Login))
            .ToList();
    }

    public async Task<GitHubPullRequestSummary?> GetPullRequestAsync(
        long installationId,
        string repositoryOwner,
        string repositoryName,
        int pullRequestNumber,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientFactory.CreateInstallationClientAsync(installationId, cancellationToken);

        try
        {
            var pullRequest = await client.PullRequest.Get(repositoryOwner, repositoryName, pullRequestNumber);
            return MapToSummary(pullRequest);
        }
        catch (NotFoundException)
        {
            return null;
        }
    }

    private static GitHubPullRequestSummary MapToSummary(PullRequest pullRequest)
    {
        return new GitHubPullRequestSummary(
            pullRequest.Id,
            pullRequest.Number,
            pullRequest.Title,
            pullRequest.HtmlUrl,
            IsOpen: pullRequest.State.Value == ItemState.Open,
            IsMerged: pullRequest.Merged,
            pullRequest.User?.Login);
    }
}
