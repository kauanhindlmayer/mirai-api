using Application.Dashboards.Queries.GetDashboard;
using Application.IntegrationTests.Infrastructure;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using FluentAssertions;

namespace Application.IntegrationTests.Dashboards.Queries;

public class GetDashboardTests : BaseIntegrationTest
{
    public GetDashboardTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetDashboard_ShouldCountOnlyClosedWorkTowardsVelocity()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);

        await SeedWorkItemAsync(team, sprint, code: 1, storyPoints: 3, Close);
        await SeedWorkItemAsync(team, sprint, code: 2, storyPoints: 5, Resolve);
        await SeedWorkItemAsync(team, sprint, code: 3, storyPoints: 8, CloseThenResolve);
        await SeedWorkItemAsync(team, sprint, code: 4, storyPoints: 13, LeaveActive);
        await SeedWorkItemAsync(team, sprint, code: 5, storyPoints: 21, CloseThenRemove);

        var query = new GetDashboardQuery(team.Id, null, null);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var velocity = result.Value.VelocityData.Single(v => v.SprintName == sprint.Name);
        velocity.CompletedStoryPoints.Should().Be(3);
        velocity.CompletedWorkItems.Should().Be(1);
    }

    [Fact]
    public async Task GetDashboard_WhenClosedWorkIsUnestimated_ShouldStillCountTheWorkItem()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);

        await SeedWorkItemAsync(team, sprint, code: 1, storyPoints: null, Close);

        var query = new GetDashboardQuery(team.Id, null, null);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        var velocity = result.Value.VelocityData.Single(v => v.SprintName == sprint.Name);
        velocity.CompletedStoryPoints.Should().Be(0);
        velocity.CompletedWorkItems.Should().Be(1);
    }

    private static void Close(WorkItem workItem) => workItem.Close();

    private static void Resolve(WorkItem workItem) =>
        workItem.Update(status: WorkItemStatus.Resolved);

    private static void CloseThenResolve(WorkItem workItem)
    {
        workItem.Close();
        workItem.Update(status: WorkItemStatus.Resolved);
    }

    private static void CloseThenRemove(WorkItem workItem)
    {
        workItem.Close();
        workItem.Update(status: WorkItemStatus.Removed);
    }

    private static void LeaveActive(WorkItem workItem) =>
        workItem.Update(status: WorkItemStatus.Active);

    private async Task<Sprint> SeedSprintAsync(Team team)
    {
        var startDate = new DateOnly(2026, 6, 1);
        var sprint = new Sprint(team.Id, $"Sprint {Guid.NewGuid()}", startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }

    private async Task SeedWorkItemAsync(
        Team team,
        Sprint sprint,
        int code,
        int? storyPoints,
        Action<WorkItem> transition)
    {
        var workItem = new WorkItem(
            team.ProjectId,
            code,
            $"Work item {code}",
            WorkItemType.UserStory,
            assignedTeamId: team.Id,
            sprintId: sprint.Id);

        workItem.Update(planning: new Planning { StoryPoints = storyPoints });
        transition(workItem);

        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
