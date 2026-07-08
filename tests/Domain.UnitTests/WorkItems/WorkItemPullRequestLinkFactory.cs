using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

internal static class WorkItemPullRequestLinkFactory
{
    public const long PullRequestId = 3003;
    public const int PullRequestNumber = 42;
    public const string Title = "Fixes #1";
    public const string HtmlUrl = "https://github.com/mirai-org/mirai-app/pull/42";
    public const string AuthorLogin = "octocat";
    public const PullRequestLinkState State = PullRequestLinkState.Open;
    public const PullRequestLinkSource Source = PullRequestLinkSource.Automatic;
    public static readonly Guid WorkItemId = Guid.NewGuid();

    public static WorkItemPullRequestLink Create(
        Guid? workItemId = null,
        long? pullRequestId = null,
        int? pullRequestNumber = null,
        string? title = null,
        string? htmlUrl = null,
        PullRequestLinkState? state = null,
        string? authorLogin = null,
        PullRequestLinkSource? source = null)
    {
        return new WorkItemPullRequestLink(
            workItemId ?? WorkItemId,
            pullRequestId ?? PullRequestId,
            pullRequestNumber ?? PullRequestNumber,
            title ?? Title,
            htmlUrl ?? HtmlUrl,
            state ?? State,
            authorLogin ?? AuthorLogin,
            source ?? Source);
    }
}
