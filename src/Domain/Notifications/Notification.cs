using Domain.Notifications.Events;
using Domain.Shared;
using Domain.Users;

namespace Domain.Notifications;

public sealed class Notification : Entity
{
    public Guid RecipientUserId { get; private set; }
    public User Recipient { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public Guid EntityId { get; private set; }
    public string Message { get; private set; } = null!;
    public DateTime? ReadAtUtc { get; private set; }

    public Notification(
        Guid recipientUserId,
        NotificationType type,
        Guid entityId,
        string message)
    {
        RecipientUserId = recipientUserId;
        Type = type;
        EntityId = entityId;
        Message = message;
        RaiseDomainEvent(new NotificationCreatedDomainEvent(this));
    }

    private Notification()
    {
    }

    public void MarkAsRead()
    {
        ReadAtUtc ??= DateTime.UtcNow;
    }
}
