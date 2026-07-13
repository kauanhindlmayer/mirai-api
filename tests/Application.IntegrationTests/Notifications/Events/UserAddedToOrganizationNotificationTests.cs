using Application.IntegrationTests.Infrastructure;
using Application.Organizations.Commands.AddUserToOrganization;
using Domain.Authorization;
using Domain.Notifications;
using Domain.Organizations;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Events;

public class UserAddedToOrganizationNotificationTests : BaseIntegrationTest
{
    public UserAddedToOrganizationNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddUserToOrganization_WhenUserAdded_ShouldCreateNotificationForAddedUser()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var owner = new User("John", "Doe", $"john-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.Add(owner);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);
        var newUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        newUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new AddUserToOrganizationCommand(organization.Id, newUser.Email);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        _dbContext.ChangeTracker.Clear();
        var notification = await _dbContext.Notifications
            .AsNoTracking()
            .SingleOrDefaultAsync(n => n.RecipientUserId == newUser.Id, TestContext.Current.CancellationToken);
        notification.Should().NotBeNull();
        notification!.Type.Should().Be(NotificationType.AddedToOrganization);
        notification.EntityId.Should().Be(organization.Id);
        notification.Message.Should().Contain(organization.Name);
    }
}
