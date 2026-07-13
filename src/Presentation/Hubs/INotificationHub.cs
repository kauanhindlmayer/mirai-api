using Application.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Presentation.Hubs;

[SignalRHub(path: "/hubs/notifications", tag: "Notifications")]
public interface INotificationHub
{
    /// <summary>
    /// Pushes a newly created notification to its recipient.
    /// </summary>
    /// <param name="notification">The notification that was created.</param>
    [SignalRMethod(name: "notification-received")]
    [HubMethodName("notification-received")]
    Task ReceiveNotification(NotificationResponse notification);
}
