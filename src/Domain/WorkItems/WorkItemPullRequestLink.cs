using Domain.Shared;
using Domain.WorkItems.Enums;

namespace Domain.WorkItems;

/// <summary>
/// Represents a link between a work item and a GitHub pull request, created
/// either automatically from a webhook-detected "#&lt;code&gt;" reference or
/// manually through the work item's UI.
/// </summary>
public sealed class WorkItemPullRequestLink : Entity
{
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public long PullRequestId { get; private set; }
    public int PullRequestNumber { get; private set; }
    public string Title { get; private set; } = null!;
    public string HtmlUrl { get; private set; } = null!;
    public PullRequestLinkState State { get; private set; }
    public string? AuthorLogin { get; private set; }
    public PullRequestLinkSource Source { get; private set; }
    public DateTime LinkedAtUtc { get; private set; }

    public WorkItemPullRequestLink(
        Guid workItemId,
        long pullRequestId,
        int pullRequestNumber,
        string title,
        string htmlUrl,
        PullRequestLinkState state,
        string? authorLogin,
        PullRequestLinkSource source)
    {
        WorkItemId = workItemId;
        PullRequestId = pullRequestId;
        PullRequestNumber = pullRequestNumber;
        Title = title;
        HtmlUrl = htmlUrl;
        State = state;
        AuthorLogin = authorLogin;
        Source = source;
        LinkedAtUtc = DateTime.UtcNow;
    }

    private WorkItemPullRequestLink()
    {
    }

    public void UpdateState(PullRequestLinkState state, string title, string htmlUrl)
    {
        State = state;
        Title = title;
        HtmlUrl = htmlUrl;
    }
}
