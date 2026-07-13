using Application.IntegrationTests.Infrastructure;
using Application.Notifications.Commands.UpdateNotificationPreferences;
using Application.Notifications.Queries.GetNotificationPreferences;
using Application.Organizations.Commands.AddUserToOrganization;
using Application.Organizations.Commands.CreateOrganization;
using Domain.Notifications;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications;

public class NotificationPreferencesTests : BaseIntegrationTest
{
    public NotificationPreferencesTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetNotificationPreferences_ForFreshUser_ShouldReturnAllEnabled()
    {
        // Arrange
        await SeedCurrentUserAsync();

        // Act
        var result = await _sender.Send(
            new GetNotificationPreferencesQuery(),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.MentionsEnabled.Should().BeTrue();
        result.Value.AssignedWorkItemChangesEnabled.Should().BeTrue();
        result.Value.WorkItemCommentsEnabled.Should().BeTrue();
        result.Value.MembershipEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateNotificationPreferences_WhenMembershipMuted_ShouldSuppressMembershipNotificationsOnly()
    {
        // Arrange
        var owner = await SeedCurrentUserAsync();
        var createResult = await _sender.Send(
            new CreateOrganizationCommand("Test Organization", "Description"),
            TestContext.Current.CancellationToken);
        var organizationId = createResult.Value;

        var newUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        newUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(newUser.Id);
        await _sender.Send(
            new UpdateNotificationPreferencesCommand(
                MentionsEnabled: true,
                AssignedWorkItemChangesEnabled: true,
                WorkItemCommentsEnabled: true,
                MembershipEnabled: false),
            TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);

        // Act
        await _sender.Send(
            new AddUserToOrganizationCommand(organizationId, newUser.Email),
            TestContext.Current.CancellationToken);

        // Assert
        _dbContext.ChangeTracker.Clear();
        var hasMembershipNotification = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(
                n => n.RecipientUserId == newUser.Id && n.Type == NotificationType.AddedToOrganization,
                TestContext.Current.CancellationToken);
        hasMembershipNotification.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateNotificationPreferences_WhenCalledTwice_ShouldUpdateExistingRowNotCreateDuplicate()
    {
        // Arrange
        await SeedCurrentUserAsync();
        await _sender.Send(
            new UpdateNotificationPreferencesCommand(
                MentionsEnabled: false,
                AssignedWorkItemChangesEnabled: true,
                WorkItemCommentsEnabled: true,
                MembershipEnabled: true),
            TestContext.Current.CancellationToken);

        // Act
        await _sender.Send(
            new UpdateNotificationPreferencesCommand(
                MentionsEnabled: true,
                AssignedWorkItemChangesEnabled: false,
                WorkItemCommentsEnabled: true,
                MembershipEnabled: true),
            TestContext.Current.CancellationToken);

        // Assert
        _dbContext.ChangeTracker.Clear();
        var rows = await _dbContext.Set<NotificationPreference>().CountAsync(TestContext.Current.CancellationToken);
        rows.Should().Be(1);

        var result = await _sender.Send(
            new GetNotificationPreferencesQuery(),
            TestContext.Current.CancellationToken);
        result.Value.MentionsEnabled.Should().BeTrue();
        result.Value.AssignedWorkItemChangesEnabled.Should().BeFalse();
    }
}
