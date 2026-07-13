using Domain.Notifications;

namespace Domain.UnitTests.Notifications;

public class NotificationTests
{
    [Fact]
    public void CreateNotification_ShouldSetProperties()
    {
        // Arrange
        var recipientUserId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var message = "You were added to the project \"Mirai\".";

        // Act
        var notification = new Notification(
            recipientUserId,
            NotificationType.AddedToProject,
            entityId,
            message);

        // Assert
        notification.RecipientUserId.Should().Be(recipientUserId);
        notification.Type.Should().Be(NotificationType.AddedToProject);
        notification.EntityId.Should().Be(entityId);
        notification.Message.Should().Be(message);
        notification.ReadAtUtc.Should().BeNull();
    }

    [Fact]
    public void MarkAsRead_WhenUnread_ShouldSetReadAtUtc()
    {
        // Arrange
        var notification = new Notification(
            Guid.NewGuid(),
            NotificationType.AddedToTeam,
            Guid.NewGuid(),
            "You were added to the team \"Platform\".");

        // Act
        notification.MarkAsRead();

        // Assert
        notification.ReadAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsRead_WhenAlreadyRead_ShouldNotChangeReadAtUtc()
    {
        // Arrange
        var notification = new Notification(
            Guid.NewGuid(),
            NotificationType.AddedToOrganization,
            Guid.NewGuid(),
            "You were added to the organization \"Mirai\".");
        notification.MarkAsRead();
        var firstReadAtUtc = notification.ReadAtUtc;

        // Act
        notification.MarkAsRead();

        // Assert
        notification.ReadAtUtc.Should().Be(firstReadAtUtc);
    }
}
