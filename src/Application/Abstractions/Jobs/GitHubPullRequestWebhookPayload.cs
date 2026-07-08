namespace Application.Abstractions.Jobs;

/// <summary>
/// The subset of a GitHub <c>pull_request</c> webhook event needed to link
/// the PR to work items, extracted at the webhook receiver so the
/// background job doesn't need to know about Octokit.Webhooks payload types.
/// </summary>
public sealed record GitHubPullRequestWebhookPayload(
    long RepositoryId,
    long PullRequestId,
    int PullRequestNumber,
    string Title,
    string Body,
    string HtmlUrl,
    string? AuthorLogin,
    string HeadBranchRef,
    bool IsClosed,
    bool IsMerged);
