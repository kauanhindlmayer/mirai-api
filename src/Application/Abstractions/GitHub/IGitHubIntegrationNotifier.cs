namespace Application.Abstractions.GitHub;

/// <summary>
/// Pushes real-time updates about GitHub pull request links. Implemented in
/// the Presentation layer (backed by SignalR), since real-time transport is
/// a presentation concern; Application/Infrastructure only see this abstraction.
/// </summary>
public interface IGitHubIntegrationNotifier
{
    Task NotifyPullRequestLinksUpdatedAsync(Guid workItemId, CancellationToken cancellationToken = default);
}
