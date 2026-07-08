using Application.Abstractions.GitHub;
using Domain.WorkItems.Events;
using MediatR;

namespace Application.WorkItems.Events;

internal sealed class PullRequestLinksUpdatedNotificationHandler
    : INotificationHandler<PullRequestLinksUpdatedDomainEvent>
{
    private readonly IGitHubIntegrationNotifier _notifier;

    public PullRequestLinksUpdatedNotificationHandler(IGitHubIntegrationNotifier notifier)
    {
        _notifier = notifier;
    }

    public Task Handle(PullRequestLinksUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return _notifier.NotifyPullRequestLinksUpdatedAsync(notification.WorkItem.Id, cancellationToken);
    }
}
