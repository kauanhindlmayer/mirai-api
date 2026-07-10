using Application.IntegrationTests.Infrastructure;
using Application.Teams.Commands.ChangeTeamMemberRole;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Teams.Commands;

public class ChangeTeamMemberRoleTests : BaseIntegrationTest
{
    public ChangeTeamMemberRoleTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ChangeTeamMemberRole_WhenMemberExists_ShouldChangeRole()
    {
        // Arrange
        var (team, _, member) = await SeedTeamWithMemberAsync();
        var command = new ChangeTeamMemberRoleCommand(
            team.Id,
            member.Id,
            SystemRoles.TeamAdminId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task ChangeTeamMemberRole_WhenDemotingLastAdmin_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var team = new Team(project.Id, "Engineering", "Description");
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Teams.Add(team);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        team.AddMember(admin, await GetRoleAsync(SystemRoles.TeamAdminId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new ChangeTeamMemberRoleCommand(
            team.Id,
            admin.Id,
            SystemRoles.TeamMemberId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.CannotRemoveLastAdmin);
    }

    private async Task<(Team Team, User Admin, User Member)> SeedTeamWithMemberAsync()
    {
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var team = new Team(project.Id, "Engineering", "Description");
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var member = new User("Member", "User", $"member-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        member.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Teams.Add(team);
        _dbContext.Users.AddRange(admin, member);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        team.AddMember(admin, await GetRoleAsync(SystemRoles.TeamAdminId));
        team.AddMember(member, await GetRoleAsync(SystemRoles.TeamMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);

        return (team, admin, member);
    }
}
