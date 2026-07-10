using Application.IntegrationTests.Infrastructure;
using Application.Projects.Commands.AddUserToProject;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Commands;

public class AddUserToProjectTests : BaseIntegrationTest
{
    public AddUserToProjectTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddUserToProject_WhenUserExists_ShouldAddAsProjectMember()
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
    }

    [Fact]
    public async Task AddUserToProject_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var existingMember = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        existingMember.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.AddRange(admin, existingMember);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(admin, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        organization.AddMember(existingMember, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));
        project.AddMember(existingMember, await GetRoleAsync(SystemRoles.ProjectContributorId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new AddUserToProjectCommand(project.Id, existingMember.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.UserAlreadyExists);
    }
}
