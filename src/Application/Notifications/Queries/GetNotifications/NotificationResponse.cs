using Domain.Notifications;

namespace Application.Notifications.Queries.GetNotifications;

public sealed class NotificationResponse
{
    public Guid Id { get; init; }
    public NotificationType Type { get; init; }
    public Guid EntityId { get; init; }
    public required string Message { get; init; }
    public DateTime? ReadAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
