using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Presentation.Hubs;

[SignalRHub(path: "/hubs/github", tag: "GitHubIntegration")]
public interface IGitHubIntegrationHub
{
    /// <summary>
    /// Notifies clients that a work item's linked pull requests changed, so
    /// an already-open work item detail view can refetch them.
    /// </summary>
    /// <param name="workItemId">The affected work item's unique identifier.</param>
    [SignalRMethod(name: "pull-request-links-updated")]
    [HubMethodName("pull-request-links-updated")]
    Task PullRequestLinksUpdated(Guid workItemId);
}
