namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to manually link a GitHub pull request to a work item.
/// </summary>
/// <param name="PullRequestNumber">The pull request's display number in the connected repository.</param>
public sealed record LinkPullRequestRequest(int PullRequestNumber);
