using Application.Abstractions.GitHub;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

internal sealed class GitHubIntegrationNotifier : IGitHubIntegrationNotifier
{
    private readonly IHubContext<GitHubIntegrationHub, IGitHubIntegrationHub> _hubContext;

    public GitHubIntegrationNotifier(IHubContext<GitHubIntegrationHub, IGitHubIntegrationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyPullRequestLinksUpdatedAsync(Guid workItemId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.PullRequestLinksUpdated(workItemId);
    }
}
