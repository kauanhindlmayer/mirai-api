using Application.IntegrationTests.Infrastructure;
using Application.Teams.Commands.AddUserToTeam;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Teams.Commands;

public class AddUserToTeamTests : BaseIntegrationTest
{
    public AddUserToTeamTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddUserToTeam_WhenUserExists_ShouldAddAsTeamMember()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var team = new Team(project.Id, "Engineering", "Description");
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var user = new User("John", "Doe", $"john-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Teams.Add(team);
        _dbContext.Users.AddRange(admin, user);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        team.AddMember(admin, await GetRoleAsync(SystemRoles.TeamAdminId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new AddUserToTeamCommand(team.Id, user.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var updatedTeam = await _dbContext.Teams.FindAsync([team.Id], TestContext.Current.CancellationToken);
        await _dbContext.Entry(updatedTeam!).Collection(t => t.Members).LoadAsync(TestContext.Current.CancellationToken);
        updatedTeam!.Members.Should().Contain(m => m.UserId == user.Id && m.RoleId == SystemRoles.TeamMemberId);
    }

    [Fact]
    public async Task AddUserToTeam_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var team = new Team(project.Id, "Engineering", "Description");
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        var existingMember = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        existingMember.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Teams.Add(team);
        _dbContext.Users.AddRange(admin, existingMember);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        team.AddMember(admin, await GetRoleAsync(SystemRoles.TeamAdminId));
        team.AddMember(existingMember, await GetRoleAsync(SystemRoles.TeamMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new AddUserToTeamCommand(team.Id, existingMember.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.UserAlreadyExists);
    }
}
