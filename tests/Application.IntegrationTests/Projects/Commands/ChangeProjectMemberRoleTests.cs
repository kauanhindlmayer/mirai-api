using Application.IntegrationTests.Infrastructure;
using Application.Projects.Commands.ChangeProjectMemberRole;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Commands;

public class ChangeProjectMemberRoleTests : BaseIntegrationTest
{
    public ChangeProjectMemberRoleTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ChangeProjectMemberRole_WhenMemberExists_ShouldChangeRole()
    {
        // Arrange
        var (project, contributor) = await SeedProjectWithMemberAsync();
        var command = new ChangeProjectMemberRoleCommand(
            project.Id,
            contributor.Id,
            SystemRoles.ProjectViewerId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task ChangeProjectMemberRole_WhenDemotingLastAdmin_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new ChangeProjectMemberRoleCommand(
            project.Id,
            admin.Id,
            SystemRoles.ProjectContributorId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.CannotRemoveLastAdmin);
    }

    private async Task<(Project Project, User Contributor)> SeedProjectWithMemberAsync()
    {
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var contributor = new User("Contributor", "User", $"contributor-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        contributor.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.AddRange(admin, contributor);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));
        project.AddMember(contributor, await GetRoleAsync(SystemRoles.ProjectContributorId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);

        return (project, contributor);
    }
}
