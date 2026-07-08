using Application.Abstractions;
using Application.Abstractions.GitHub;
using Domain.Projects;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs;

/// <summary>
/// Recurring safety net over the webhook path: re-fetches state for
/// non-terminal (Open) pull request links to catch webhook deliveries that
/// never arrived, and does a best-effort "recently updated" title scan per
/// connected repository to catch entirely missed "opened" events. Not
/// required for the feature to work correctly - it's a fallback only.
/// </summary>
public sealed class GitHubRepositorySyncJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly IGitHubPullRequestService _pullRequestService;
    private readonly ILogger<GitHubRepositorySyncJob> _logger;

    public GitHubRepositorySyncJob(
        IApplicationDbContext context,
        IGitHubPullRequestService pullRequestService,
        ILogger<GitHubRepositorySyncJob> logger)
    {
        _context = context;
        _pullRequestService = pullRequestService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var connectedProjects = await _context.Projects
            .Include(p => p.GitHubRepositoryConnection)
            .Where(p => p.GitHubRepositoryConnection != null)
            .ToListAsync(context.CancellationToken);

        _logger.LogInformation(
            "GitHub repository sync started for {ProjectCount} connected project(s).",
            connectedProjects.Count);

        foreach (var project in connectedProjects)
        {
            await SyncOpenPullRequestLinksAsync(project, context.CancellationToken);
            await SyncRecentlyUpdatedPullRequestsAsync(project, context.CancellationToken);

            project.GitHubRepositoryConnection!.UpdateLastSyncedAt(DateTime.UtcNow);
        }

        await _context.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("GitHub repository sync completed.");
    }

    private async Task SyncOpenPullRequestLinksAsync(Project project, CancellationToken cancellationToken)
    {
        var connection = project.GitHubRepositoryConnection!;

        var workItemsWithOpenLinks = await _context.WorkItems
            .Include(wi => wi.PullRequestLinks)
            .Where(wi => wi.ProjectId == project.Id
                && wi.PullRequestLinks.Any(l => l.State == PullRequestLinkState.Open))
            .ToListAsync(cancellationToken);

        if (workItemsWithOpenLinks.Count == 0)
        {
            return;
        }

        var checkedCount = 0;
        var updatedCount = 0;

        foreach (var workItem in workItemsWithOpenLinks)
        {
            var openLinks = workItem.PullRequestLinks
                .Where(l => l.State == PullRequestLinkState.Open)
                .ToList();

            foreach (var link in openLinks)
            {
                checkedCount++;

                var pullRequest = await TryGetPullRequestAsync(connection, link.PullRequestNumber, cancellationToken);
                if (pullRequest is null)
                {
                    continue;
                }

                workItem.UpsertPullRequestLink(
                    pullRequest.Id,
                    pullRequest.Number,
                    pullRequest.Title,
                    pullRequest.HtmlUrl,
                    ResolveState(pullRequest),
                    pullRequest.AuthorLogin,
                    link.Source);

                updatedCount++;
            }
        }

        _logger.LogInformation(
            "Re-checked {CheckedCount} open pull request link(s) for {Owner}/{Name}, updated {UpdatedCount}.",
            checkedCount,
            connection.RepositoryOwner,
            connection.RepositoryName,
            updatedCount);
    }

    private async Task SyncRecentlyUpdatedPullRequestsAsync(Project project, CancellationToken cancellationToken)
    {
        var connection = project.GitHubRepositoryConnection!;
        var sinceUtc = connection.LastSyncedAtUtc ?? connection.ConnectedAtUtc;
        var searchQuery = $"updated:>{sinceUtc:yyyy-MM-dd}";

        IReadOnlyList<GitHubPullRequestSummary> recentPullRequests;
        try
        {
            recentPullRequests = await _pullRequestService.SearchPullRequestsAsync(
                connection.InstallationId,
                connection.RepositoryOwner,
                connection.RepositoryName,
                searchQuery,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to search recently updated GitHub pull requests for {Owner}/{Name}.",
                connection.RepositoryOwner,
                connection.RepositoryName);
            return;
        }

        if (recentPullRequests.Count == 0)
        {
            return;
        }

        var linkedCount = 0;

        foreach (var pullRequest in recentPullRequests)
        {
            // Title-only: search results don't include the PR body, so a
            // reference mentioned only in the body waits for the next real
            // webhook delivery. Good enough for a fallback safety net.
            var codes = PullRequestReferenceParser.ParseCodes(pullRequest.Title);
            foreach (var code in codes)
            {
                var workItem = await _context.WorkItems
                    .Include(wi => wi.PullRequestLinks)
                    .FirstOrDefaultAsync(wi => wi.ProjectId == project.Id && wi.Code == code, cancellationToken);

                if (workItem is null)
                {
                    continue;
                }

                workItem.UpsertPullRequestLink(
                    pullRequest.Id,
                    pullRequest.Number,
                    pullRequest.Title,
                    pullRequest.HtmlUrl,
                    ResolveState(pullRequest),
                    pullRequest.AuthorLogin,
                    PullRequestLinkSource.Automatic);

                linkedCount++;
            }
        }

        _logger.LogInformation(
            "Scanned {PullRequestCount} recently updated pull request(s) for {Owner}/{Name}, linked {LinkedCount}.",
            recentPullRequests.Count,
            connection.RepositoryOwner,
            connection.RepositoryName,
            linkedCount);
    }

    private async Task<GitHubPullRequestSummary?> TryGetPullRequestAsync(
        GitHubRepositoryConnection connection,
        int pullRequestNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _pullRequestService.GetPullRequestAsync(
                connection.InstallationId,
                connection.RepositoryOwner,
                connection.RepositoryName,
                pullRequestNumber,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to sync GitHub pull request #{PullRequestNumber} for {Owner}/{Name}.",
                pullRequestNumber,
                connection.RepositoryOwner,
                connection.RepositoryName);
            return null;
        }
    }

    private static PullRequestLinkState ResolveState(GitHubPullRequestSummary pullRequest)
    {
        if (pullRequest.IsMerged)
        {
            return PullRequestLinkState.Merged;
        }

        return pullRequest.IsOpen ? PullRequestLinkState.Open : PullRequestLinkState.Closed;
    }
}
