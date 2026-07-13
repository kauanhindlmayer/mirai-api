using Application.Abstractions.Notifications;
using Application.Notifications.Events;
using Application.Notifications.Queries.GetNotifications;
using Domain.Notifications;
using Domain.Notifications.Events;

namespace Application.UnitTests.Notifications.Events;

public class NotificationCreatedRealtimeHandlerTests
{
    private readonly INotificationRealtimeNotifier _notifier;
    private readonly NotificationCreatedRealtimeHandler _handler;

    public NotificationCreatedRealtimeHandlerTests()
    {
        _notifier = Substitute.For<INotificationRealtimeNotifier>();
        _handler = new NotificationCreatedRealtimeHandler(_notifier);
    }

    [Fact]
    public async Task Handle_ShouldNotifyRecipientWithMappedNotification()
    {
        // Arrange
        var recipientId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        const string message = "You were added to the project \"Mirai\".";
        var notification = new Notification(
            recipientId,
            NotificationType.AddedToProject,
            entityId,
            message);
        var domainEvent = new NotificationCreatedDomainEvent(notification);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        await _notifier.Received(1).NotifyNotificationCreatedAsync(
            recipientId,
            Arg.Is<NotificationResponse>(r =>
                r.Id == notification.Id &&
                r.Type == NotificationType.AddedToProject &&
                r.EntityId == entityId &&
                r.Message == message &&
                r.ReadAtUtc == null),
            TestContext.Current.CancellationToken);
    }
}
