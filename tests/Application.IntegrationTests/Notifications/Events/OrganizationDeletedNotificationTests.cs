using Application.IntegrationTests.Infrastructure;
using Application.Organizations.Commands.AddUserToOrganization;
using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Events;

public class OrganizationDeletedNotificationTests : BaseIntegrationTest
{
    public OrganizationDeletedNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteOrganization_ShouldRemoveAddedToOrganizationNotifications()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var createResult = await _sender.Send(
            new CreateOrganizationCommand("Test Organization", "Test Description"),
            TestContext.Current.CancellationToken);
        var organizationId = createResult.Value;

        var newUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        newUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _sender.Send(
            new AddUserToOrganizationCommand(organizationId, newUser.Email),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();
        var notificationExisted = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(n => n.RecipientUserId == newUser.Id, TestContext.Current.CancellationToken);
        notificationExisted.Should().BeTrue();

        // Act
        var deleteResult = await _sender.Send(
            new DeleteOrganizationCommand(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        deleteResult.IsError.Should().BeFalse();

        _dbContext.ChangeTracker.Clear();
        var notificationStillExists = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(n => n.RecipientUserId == newUser.Id, TestContext.Current.CancellationToken);
        notificationStillExists.Should().BeFalse();
    }
}
