using Application.Abstractions.Notifications;
using Application.Notifications.Queries.GetNotifications;
using Domain.Notifications.Events;
using MediatR;

namespace Application.Notifications.Events;

internal sealed class NotificationCreatedRealtimeHandler
    : INotificationHandler<NotificationCreatedDomainEvent>
{
    private readonly INotificationRealtimeNotifier _notifier;

    public NotificationCreatedRealtimeHandler(INotificationRealtimeNotifier notifier)
    {
        _notifier = notifier;
    }

    public Task Handle(
        NotificationCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var response = new NotificationResponse
        {
            Id = notification.Notification.Id,
            Type = notification.Notification.Type,
            EntityId = notification.Notification.EntityId,
            Message = notification.Notification.Message,
            ReadAtUtc = notification.Notification.ReadAtUtc,
            CreatedAtUtc = notification.Notification.CreatedAtUtc,
        };

        return _notifier.NotifyNotificationCreatedAsync(
            notification.Notification.RecipientUserId,
            response,
            cancellationToken);
    }
}
