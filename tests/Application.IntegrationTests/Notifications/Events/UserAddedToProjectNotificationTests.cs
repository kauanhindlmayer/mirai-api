using Application.IntegrationTests.Infrastructure;
using Application.Projects.Commands.AddUserToProject;
using Domain.Authorization;
using Domain.Notifications;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Events;

public class UserAddedToProjectNotificationTests : BaseIntegrationTest
{
    public UserAddedToProjectNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddUserToProject_WhenUserAdded_ShouldCreateNotificationForAddedUser()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var user = new User("John", "Doe", $"john-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.AddRange(admin, user);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(admin, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        organization.AddMember(user, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new AddUserToProjectCommand(project.Id, user.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        _dbContext.ChangeTracker.Clear();
        var notification = await _dbContext.Notifications
            .AsNoTracking()
            .SingleOrDefaultAsync(n => n.RecipientUserId == user.Id, TestContext.Current.CancellationToken);
        notification.Should().NotBeNull();
        notification!.Type.Should().Be(NotificationType.AddedToProject);
        notification.EntityId.Should().Be(project.Id);
        notification.Message.Should().Contain(project.Name);
    }
}
