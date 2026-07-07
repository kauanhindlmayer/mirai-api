using Application.Abstractions;
using Application.Abstractions.Jobs;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.Installation;
using Octokit.Webhooks.Events.InstallationRepositories;
using Octokit.Webhooks.Events.PullRequest;

namespace Infrastructure.Integrations.GitHub;

/// <summary>
/// Handles GitHub App webhook deliveries. Signature verification and payload
/// deserialization/dispatch are handled by Octokit.Webhooks.AspNetCore's
/// <see cref="WebhookEventProcessor"/> base class and its
/// <c>MapGitHubWebhooks</c> endpoint mapping, so this class only needs to
/// react to the events Mirai cares about.
/// </summary>
internal sealed class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IBackgroundJobScheduler _jobScheduler;
    private readonly IApplicationDbContext _context;

    public GitHubWebhookEventProcessor(
        IBackgroundJobScheduler jobScheduler,
        IApplicationDbContext context)
    {
        _jobScheduler = jobScheduler;
        _context = context;
    }

    protected override async ValueTask ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action,
        CancellationToken cancellationToken = default)
    {
        if (!IsHandledAction(action) || pullRequestEvent.Repository is null)
        {
            return;
        }

        var pullRequest = pullRequestEvent.PullRequest;

        var payload = new GitHubPullRequestWebhookPayload(
            pullRequestEvent.Repository.Id,
            pullRequest.Id,
            (int)pullRequest.Number,
            pullRequest.Title,
            pullRequest.Body ?? string.Empty,
            pullRequest.HtmlUrl,
            pullRequest.User?.Login,
            pullRequest.Head.Ref,
            action == PullRequestAction.Closed,
            pullRequest.Merged ?? false);

        await _jobScheduler.ScheduleGitHubPullRequestWebhookJobAsync(payload, cancellationToken);
    }

    protected override async ValueTask ProcessInstallationWebhookAsync(
        WebhookHeaders headers,
        InstallationEvent installationEvent,
        InstallationAction action,
        CancellationToken cancellationToken = default)
    {
        if (action != InstallationAction.Deleted)
        {
            return;
        }

        var projects = _context.Projects
            .Where(p => p.GitHubRepositoryConnection != null
                && p.GitHubRepositoryConnection.InstallationId == installationEvent.Installation.Id)
            .ToList();

        foreach (var project in projects)
        {
            project.DisconnectGitHubRepository();
        }

        if (projects.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    protected override async ValueTask ProcessInstallationRepositoriesWebhookAsync(
        WebhookHeaders headers,
        InstallationRepositoriesEvent installationRepositoriesEvent,
        InstallationRepositoriesAction action,
        CancellationToken cancellationToken = default)
    {
        if (action != InstallationRepositoriesAction.Removed)
        {
            return;
        }

        var removedRepositoryIds = installationRepositoriesEvent.RepositoriesRemoved
            .Select(repository => repository.Id)
            .ToList();

        var projects = _context.Projects
            .Where(p => p.GitHubRepositoryConnection != null
                && removedRepositoryIds.Contains(p.GitHubRepositoryConnection.RepositoryId))
            .ToList();

        foreach (var project in projects)
        {
            project.DisconnectGitHubRepository();
        }

        if (projects.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static bool IsHandledAction(PullRequestAction action)
    {
        return action == PullRequestAction.Opened
            || action == PullRequestAction.Edited
            || action == PullRequestAction.Synchronize
            || action == PullRequestAction.Reopened
            || action == PullRequestAction.Closed;
    }
}
