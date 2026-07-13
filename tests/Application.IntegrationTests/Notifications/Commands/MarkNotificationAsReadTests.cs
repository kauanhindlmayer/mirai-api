using Application.IntegrationTests.Infrastructure;
using Application.Notifications.Commands.MarkNotificationAsRead;
using Domain.Notifications;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Commands;

public class MarkNotificationAsReadTests : BaseIntegrationTest
{
    public MarkNotificationAsReadTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MarkNotificationAsRead_WhenOwnedByCurrentUser_ShouldMarkAsRead()
    {
        // Arrange
        var user = await SeedCurrentUserAsync();
        var notification = new Notification(
            user.Id,
            NotificationType.AddedToProject,
            Guid.NewGuid(),
            "You were added to the project \"Mirai\".");
        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        var command = new MarkNotificationAsReadCommand(notification.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var updated = await _dbContext.Notifications
            .AsNoTracking()
            .SingleAsync(n => n.Id == notification.Id, TestContext.Current.CancellationToken);
        updated.ReadAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkNotificationAsRead_WhenOwnedByAnotherUser_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var otherUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        otherUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(otherUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var notification = new Notification(
            otherUser.Id,
            NotificationType.AddedToProject,
            Guid.NewGuid(),
            "You were added to the project \"Mirai\".");
        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        var command = new MarkNotificationAsReadCommand(notification.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(NotificationErrors.NotFound);
    }
}
