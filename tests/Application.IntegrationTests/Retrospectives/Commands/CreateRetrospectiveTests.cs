using Application.IntegrationTests.Infrastructure;
using Application.Retrospectives.Commands.CreateRetrospective;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Retrospectives.Enums;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Retrospectives.Commands;

public class CreateRetrospectiveTests : BaseIntegrationTest
{
    public CreateRetrospectiveTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateRetrospective_WhenRetrospectiveAlreadyExists_ShouldReturnError()
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

        var existingRetrospective = new Retrospective("Sprint Review", team.Id, 5, null);
        team.AddRetrospective(existingRetrospective);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new CreateRetrospectiveCommand(
            "Sprint Review",
            5,
            RetrospectiveTemplate.StartStopContinue,
            team.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.AlreadyExists);
    }
}
