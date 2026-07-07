using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

/// <summary>
/// Pure server-push hub: the webhook job and manual-link handlers push
/// updates via <see cref="GitHubIntegrationNotifier"/>; clients only ever
/// subscribe, they don't invoke methods on this hub.
/// </summary>
public sealed class GitHubIntegrationHub : Hub<IGitHubIntegrationHub>
{
}
