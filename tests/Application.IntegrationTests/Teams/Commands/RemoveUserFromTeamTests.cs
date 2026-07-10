using Application.Abstractions.Authorization;
using Application.IntegrationTests.Infrastructure;
using Application.Teams.Commands.RemoveUserFromTeam;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Teams.Commands;

public class RemoveUserFromTeamTests : BaseIntegrationTest
{
    public RemoveUserFromTeamTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task RemoveUserFromTeam_WhenUserIsMember_ShouldRemoveUser()
    {
        // Arrange
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
        var command = new RemoveUserFromTeamCommand(team.Id, member.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveUserFromTeam_WhenTeamDoesNotExist_ShouldReturnForbidden()
    {
        // Arrange
        var command = new RemoveUserFromTeamCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        // Authorization is checked before the handler can determine the team does not
        // exist, so the caller - who has no membership in a nonexistent team - is
        // denied access rather than told it is missing.
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthorizationErrors.Forbidden);
    }
}
