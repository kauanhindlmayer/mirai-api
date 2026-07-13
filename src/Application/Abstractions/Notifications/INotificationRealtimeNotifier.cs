using Application.Notifications.Queries.GetNotifications;

namespace Application.Abstractions.Notifications;

/// <summary>
/// Pushes a newly created notification to its recipient in real time. Implemented in
/// the Presentation layer (backed by SignalR), since real-time transport is a
/// presentation concern; Application/Infrastructure only see this abstraction.
/// </summary>
public interface INotificationRealtimeNotifier
{
    Task NotifyNotificationCreatedAsync(
        Guid recipientUserId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default);
}
