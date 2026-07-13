using Application.IntegrationTests.Infrastructure;
using Application.Notifications.Queries.GetNotifications;
using Domain.Notifications;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Notifications.Queries;

public class GetNotificationsTests : BaseIntegrationTest
{
    public GetNotificationsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnOnlyCurrentUsersNotificationsNewestFirst()
    {
        // Arrange
        var user = await SeedCurrentUserAsync();
        var otherUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        otherUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(otherUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var older = new Notification(
            user.Id, NotificationType.AddedToProject, Guid.NewGuid(), "Older notification.");
        _dbContext.Notifications.Add(older);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var newer = new Notification(
            user.Id, NotificationType.AddedToTeam, Guid.NewGuid(), "Newer notification.");
        _dbContext.Notifications.Add(newer);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var otherUsersNotification = new Notification(
            otherUser.Id, NotificationType.AddedToOrganization, Guid.NewGuid(), "Other user's notification.");
        _dbContext.Notifications.Add(otherUsersNotification);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        var query = new GetNotificationsQuery(UnreadOnly: false, Page: 1, PageSize: 10);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Select(n => n.Id).Should().ContainInOrder(newer.Id, older.Id);
        result.Value.Items.Should().NotContain(n => n.Id == otherUsersNotification.Id);
    }

    [Fact]
    public async Task GetNotifications_WhenUnreadOnly_ShouldExcludeReadNotifications()
    {
        // Arrange
        var user = await SeedCurrentUserAsync();
        var unread = new Notification(
            user.Id, NotificationType.AddedToProject, Guid.NewGuid(), "Unread notification.");
        var read = new Notification(
            user.Id, NotificationType.AddedToTeam, Guid.NewGuid(), "Read notification.");
        read.MarkAsRead();
        _dbContext.Notifications.AddRange(unread, read);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        _dbContext.ChangeTracker.Clear();

        var query = new GetNotificationsQuery(UnreadOnly: true, Page: 1, PageSize: 10);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().ContainSingle(n => n.Id == unread.Id);
    }
}
