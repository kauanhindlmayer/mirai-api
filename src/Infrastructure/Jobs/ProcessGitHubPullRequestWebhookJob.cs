using Application.Abstractions;
using Application.Abstractions.Jobs;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs;

public sealed class ProcessGitHubPullRequestWebhookJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ProcessGitHubPullRequestWebhookJob> _logger;

    public ProcessGitHubPullRequestWebhookJob(
        IApplicationDbContext context,
        ILogger<ProcessGitHubPullRequestWebhookJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var payload = (GitHubPullRequestWebhookPayload)context.MergedJobDataMap["payload"];

        var project = _context.Projects
            .FirstOrDefault(p => p.GitHubRepositoryConnection != null
                && p.GitHubRepositoryConnection.RepositoryId == payload.RepositoryId);

        if (project is null)
        {
            _logger.LogWarning(
                "Received a GitHub pull request webhook for repository {RepositoryId}, which has no connected project.",
                payload.RepositoryId);
            return;
        }

        var codes = PullRequestReferenceParser.ParseCodes(payload.Title, payload.Body, payload.HeadBranchRef);
        if (codes.Count == 0)
        {
            return;
        }

        var state = ResolveState(payload);
        var linkedAnyWorkItem = false;

        foreach (var code in codes)
        {
            var workItem = _context.WorkItems
                .FirstOrDefault(wi => wi.ProjectId == project.Id && wi.Code == code);

            if (workItem is null)
            {
                continue;
            }

            workItem.UpsertPullRequestLink(
                payload.PullRequestId,
                payload.PullRequestNumber,
                payload.Title,
                payload.HtmlUrl,
                state,
                payload.AuthorLogin,
                PullRequestLinkSource.Automatic);

            linkedAnyWorkItem = true;
        }

        if (linkedAnyWorkItem)
        {
            await _context.SaveChangesAsync(context.CancellationToken);
        }
    }

    private static PullRequestLinkState ResolveState(GitHubPullRequestWebhookPayload payload)
    {
        if (!payload.IsClosed)
        {
            return PullRequestLinkState.Open;
        }

        return payload.IsMerged ? PullRequestLinkState.Merged : PullRequestLinkState.Closed;
    }
}
