using Application.IntegrationTests.Infrastructure;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Authorization;

namespace Application.IntegrationTests.Authorization;

public class PermissionServiceTests : BaseIntegrationTest
{
    private readonly PermissionService _permissionService;

    public PermissionServiceTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _permissionService = new PermissionService(_dbContext);
    }

    [Fact]
    public async Task HasPermissionAsync_WhenUserIsOrganizationOwner_ShouldGrantProjectScopedPermission()
    {
        // Arrange
        var (organization, project, _, owner) = await SeedHierarchyAsync(
            SystemRoles.OrganizationOwnerId,
            SystemRoles.ProjectViewerId,
            SystemRoles.TeamMemberId);

        // Act
        var result = await _permissionService.HasPermissionAsync(
            owner.Id,
            Permission.ProjectManage,
            ResourceType.Project,
            project.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
        organization.Should().NotBeNull();
    }

    [Fact]
    public async Task HasPermissionAsync_WhenUserHasProjectRoleGrantingPermission_ShouldReturnTrue()
    {
        // Arrange
        var (_, project, _, admin) = await SeedHierarchyAsync(
            SystemRoles.OrganizationMemberId,
            SystemRoles.ProjectAdminId,
            SystemRoles.TeamMemberId);

        // Act
        var result = await _permissionService.HasPermissionAsync(
            admin.Id,
            Permission.ProjectManageMembers,
            ResourceType.Project,
            project.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPermissionAsync_WhenUserHasNoRoleGrantingPermission_ShouldReturnFalse()
    {
        // Arrange
        var (_, project, _, viewer) = await SeedHierarchyAsync(
            SystemRoles.OrganizationMemberId,
            SystemRoles.ProjectViewerId,
            SystemRoles.TeamMemberId);

        // Act
        var result = await _permissionService.HasPermissionAsync(
            viewer.Id,
            Permission.ProjectManageMembers,
            ResourceType.Project,
            project.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasPermissionAsync_WhenUserHasTeamRoleGrantingPermission_ShouldReturnTrue()
    {
        // Arrange
        var (_, _, team, teamAdmin) = await SeedHierarchyAsync(
            SystemRoles.OrganizationMemberId,
            SystemRoles.ProjectViewerId,
            SystemRoles.TeamAdminId);

        // Act
        var result = await _permissionService.HasPermissionAsync(
            teamAdmin.Id,
            Permission.TeamManageMembers,
            ResourceType.Team,
            team.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPermissionAsync_WhenUserIsNotAMember_ShouldReturnFalse()
    {
        // Arrange
        var (_, project, _, _) = await SeedHierarchyAsync(
            SystemRoles.OrganizationOwnerId,
            SystemRoles.ProjectAdminId,
            SystemRoles.TeamAdminId);
        var strangerId = Guid.NewGuid();

        // Act
        var result = await _permissionService.HasPermissionAsync(
            strangerId,
            Permission.ProjectView,
            ResourceType.Project,
            project.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    private async Task<(Organization Organization, Project Project, Team Team, User User)> SeedHierarchyAsync(
        Guid organizationRoleId,
        Guid projectRoleId,
        Guid teamRoleId)
    {
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        _dbContext.Organizations.Add(organization);

        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        _dbContext.Projects.Add(project);

        var team = new Team(project.Id, "Engineering", "Description");
        _dbContext.Teams.Add(team);

        var user = new User("Test", "User", $"user-{Guid.NewGuid()}@mirai.com");
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(user, await GetRoleAsync(organizationRoleId));
        project.AddMember(user, await GetRoleAsync(projectRoleId));
        team.AddMember(user, await GetRoleAsync(teamRoleId));

        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (organization, project, team, user);
    }
}
