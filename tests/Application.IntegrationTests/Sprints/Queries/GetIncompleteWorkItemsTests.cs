using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Queries.GetIncompleteWorkItems;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.Sprints.Queries;

public class GetIncompleteWorkItemsTests : BaseIntegrationTest
{
    public GetIncompleteWorkItemsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetIncompleteWorkItems_WhenSprintDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();

        var query = new GetIncompleteWorkItemsQuery(team.Id, Guid.NewGuid());

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotFound);
    }

    [Fact]
    public async Task GetIncompleteWorkItems_WhenSprintBelongsToAnotherTeam_ShouldReturnNotFound()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var otherTeam = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);

        var query = new GetIncompleteWorkItemsQuery(otherTeam.Id, sprint.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task GetIncompleteWorkItems_ShouldReturnExactlyTheSprintsIncompleteWorkItems()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);
        var incompleteStory = await SeedWorkItemAsync(
            team,
            sprint,
            code: 1,
            "Incomplete story",
            WorkItemType.UserStory,
            WorkItemStatus.New);

        // A standalone Bug assigned directly to the sprint, with no parent story
        // in this sprint - excluded from the UserStory-level backlog view, but
        // still part of what Sprint.IncompleteWorkItems() returns.
        var standaloneBug = await SeedWorkItemAsync(
            team,
            sprint,
            code: 2,
            "Standalone bug",
            WorkItemType.Bug,
            WorkItemStatus.New);

        await SeedWorkItemAsync(
            team,
            sprint,
            code: 3,
            "Closed story",
            WorkItemType.UserStory,
            WorkItemStatus.Closed);

        await SeedWorkItemAsync(
            team,
            sprint,
            code: 4,
            "Removed defect",
            WorkItemType.Defect,
            WorkItemStatus.Removed);

        _dbContext.ChangeTracker.Clear();

        var query = new GetIncompleteWorkItemsQuery(team.Id, sprint.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.Select(wi => wi.Id).Should().BeEquivalentTo(
            [incompleteStory.Id, standaloneBug.Id]);
        var bug = result.Value.Single(wi => wi.Id == standaloneBug.Id);
        bug.Code.Should().Be(2);
        bug.Title.Should().Be("Standalone bug");
        bug.Type.Should().Be(nameof(WorkItemType.Bug));
        bug.Status.Should().Be(nameof(WorkItemStatus.New));
    }

    private async Task<Sprint> SeedSprintAsync(Team team)
    {
        var startDate = new DateOnly(2026, 6, 1);
        var sprint = new Sprint(team.Id, "Sprint 1", startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }

    private async Task<WorkItem> SeedWorkItemAsync(
        Team team,
        Sprint sprint,
        int code,
        string title,
        WorkItemType type,
        WorkItemStatus status)
    {
        var workItem = new WorkItem(
            team.ProjectId,
            code,
            title,
            type,
            assignedTeamId: team.Id,
            sprintId: sprint.Id);
        SetStatus(workItem, status);
        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return workItem;
    }

    private static void SetStatus(WorkItem workItem, WorkItemStatus status)
    {
        if (status == WorkItemStatus.Closed)
        {
            workItem.Close();
            return;
        }

        if (status != WorkItemStatus.New)
        {
            workItem.Update(status: status);
        }
    }
}
