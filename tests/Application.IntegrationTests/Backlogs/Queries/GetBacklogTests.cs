using Application.Backlogs.Queries.GetBacklog;
using Application.IntegrationTests.Infrastructure;
using Domain.Backlogs;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Backlogs.Queries;

public class GetBacklogTests : BaseIntegrationTest
{
    public GetBacklogTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetBacklog_WhenNoSprintIsGiven_ShouldReturnOnlyWorkItemsInNoSprint()
    {
        // Arrange
        var (team, sprint) = await SeedTeamWithSprintAsync();
        await SeedWorkItemAsync(team, code: 1, "In the sprint", sprint.Id);
        await SeedWorkItemAsync(team, code: 2, "In the backlog", sprintId: null);

        var query = new GetBacklogQuery(team.Id, null, BacklogLevel.UserStory);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Select(wi => wi.Title).Should().BeEquivalentTo("In the backlog");
    }

    [Fact]
    public async Task GetBacklog_WhenSprintIsGiven_ShouldReturnOnlyThatSprintsWorkItems()
    {
        // Arrange
        var (team, sprint) = await SeedTeamWithSprintAsync();
        await SeedWorkItemAsync(team, code: 1, "In the sprint", sprint.Id);
        await SeedWorkItemAsync(team, code: 2, "In the backlog", sprintId: null);

        var query = new GetBacklogQuery(team.Id, sprint.Id, BacklogLevel.UserStory);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Select(wi => wi.Title).Should().BeEquivalentTo("In the sprint");
    }

    private async Task<(Team Team, Sprint Sprint)> SeedTeamWithSprintAsync()
    {
        var team = await SeedTeamWithAdminAsync();

        var startDate = new DateOnly(2026, 6, 1);
        var sprint = new Sprint(team.Id, "Sprint 1", startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (team, sprint);
    }

    private async Task SeedWorkItemAsync(
        Team team,
        int code,
        string title,
        Guid? sprintId)
    {
        var workItem = new WorkItem(
            team.ProjectId,
            code,
            title,
            WorkItemType.UserStory,
            assignedTeamId: team.Id,
            sprintId: sprintId);

        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
