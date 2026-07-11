using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.AddWorkItemToSprint;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Sprints;
using Domain.Teams;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Sprints.Commands;

public class AddWorkItemToSprintTests : BaseIntegrationTest
{
    public AddWorkItemToSprintTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddWorkItemToSprint_WhenWorkItemAlreadyInSprint_ShouldReturnError()
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

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var sprint = new Sprint(team.Id, "Sprint 1", startDate, startDate.AddDays(14));
        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.Sprints.Add(sprint);
        _dbContext.WorkItems.Add(workItem);
        sprint.AddWorkItem(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new AddWorkItemToSprintCommand(team.Id, sprint.Id, workItem.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.WorkItemAlreadyInSprint);
    }
}
