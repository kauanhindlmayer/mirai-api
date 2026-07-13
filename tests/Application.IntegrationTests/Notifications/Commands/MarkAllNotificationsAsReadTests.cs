using Application.IntegrationTests.Infrastructure;
using Application.Notifications.Commands.MarkAllNotificationsAsRead;
using Domain.Notifications;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Commands;

public class MarkAllNotificationsAsReadTests : BaseIntegrationTest
{
    public MarkAllNotificationsAsReadTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MarkAllNotificationsAsRead_ShouldMarkOnlyCurrentUsersUnreadNotifications()
    {
        // Arrange
        var user = await SeedCurrentUserAsync();
        var otherUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        otherUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(otherUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var firstUnread = new Notification(
            user.Id, NotificationType.AddedToProject, Guid.NewGuid(), "First notification.");
        var secondUnread = new Notification(
            user.Id, NotificationType.AddedToTeam, Guid.NewGuid(), "Second notification.");
        var otherUsersNotification = new Notification(
            otherUser.Id, NotificationType.AddedToOrganization, Guid.NewGuid(), "Other user's notification.");
        _dbContext.Notifications.AddRange(firstUnread, secondUnread, otherUsersNotification);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        var command = new MarkAllNotificationsAsReadCommand();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var notifications = await _dbContext.Notifications
            .AsNoTracking()
            .ToDictionaryAsync(n => n.Id, TestContext.Current.CancellationToken);
        notifications[firstUnread.Id].ReadAtUtc.Should().NotBeNull();
        notifications[secondUnread.Id].ReadAtUtc.Should().NotBeNull();
        notifications[otherUsersNotification.Id].ReadAtUtc.Should().BeNull();
    }
}
