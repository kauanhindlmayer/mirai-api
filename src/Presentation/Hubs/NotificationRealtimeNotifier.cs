using Application.Abstractions.Notifications;
using Application.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

internal sealed class NotificationRealtimeNotifier : INotificationRealtimeNotifier
{
    private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

    public NotificationRealtimeNotifier(IHubContext<NotificationHub, INotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyNotificationCreatedAsync(
        Guid recipientUserId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients
            .User(recipientUserId.ToString())
            .ReceiveNotification(notification);
    }
}
