using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.CompleteSprint;
using Application.Sprints.Commands.StartSprint;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Sprints.Commands;

public class CompleteSprintTests : BaseIntegrationTest
{
    public CompleteSprintTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CompleteSprint_WhenSprintIsNotActive_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new CompleteSprintCommand(team.Id, sprint.Id, []),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotActive);
    }

    [Fact]
    public async Task CompleteSprint_WhenAnIncompleteItemHasNoDisposition_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        await SeedWorkItemInSprintAsync(team, sprint, code: 1, storyPoints: 5);
        await StartAsync(team, sprint);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new CompleteSprintCommand(team.Id, sprint.Id, []),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.DispositionsIncomplete);
    }

    [Fact]
    public async Task CompleteSprint_ShouldApplyDispositionsAndFreezeDeliveredFigures()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var next = await SeedSprintAsync(team, "Sprint 2", new DateOnly(2026, 6, 15));
        var delivered = await SeedWorkItemInSprintAsync(team, sprint, code: 1, storyPoints: 5);
        var carried = await SeedWorkItemInSprintAsync(team, sprint, code: 2, storyPoints: 8);
        var backlogged = await SeedWorkItemInSprintAsync(team, sprint, code: 3, storyPoints: 3);
        await StartAsync(team, sprint);
        await CloseAsync(delivered);
        var dispositions = new List<SprintItemDisposition>
        {
            new(carried.Id, DispositionTarget.Sprint, next.Id),
            new(backlogged.Id, DispositionTarget.Backlog, null),
        };

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new CompleteSprintCommand(team.Id, sprint.Id, dispositions),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var completedSprint = await _dbContext.Sprints
            .AsNoTracking()
            .FirstAsync(s => s.Id == sprint.Id, TestContext.Current.CancellationToken);
        completedSprint.Status.Should().Be(SprintStatus.Completed);
        completedSprint.CompletedAtUtc.Should().NotBeNull();
        completedSprint.CompletedStoryPoints.Should().Be(5);
        completedSprint.CompletedWorkItemCount.Should().Be(1);

        var workItems = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.AssignedTeamId == team.Id)
            .ToListAsync(TestContext.Current.CancellationToken);
        workItems.Single(wi => wi.Id == delivered.Id).SprintId.Should().Be(sprint.Id);
        workItems.Single(wi => wi.Id == carried.Id).SprintId.Should().Be(next.Id);
        workItems.Single(wi => wi.Id == backlogged.Id).SprintId.Should().BeNull();
    }

    private async Task<Sprint> SeedSprintAsync(Team team, string name, DateOnly startDate)
    {
        var sprint = new Sprint(team.Id, name, startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }

    private async Task<WorkItem> SeedWorkItemInSprintAsync(
        Team team,
        Sprint sprint,
        int code,
        int? storyPoints)
    {
        var workItem = new WorkItem(
            team.ProjectId,
            code,
            $"Work item {code}",
            WorkItemType.UserStory,
            assignedTeamId: team.Id,
            sprintId: sprint.Id);
        workItem.Update(planning: new Planning { StoryPoints = storyPoints });
        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return workItem;
    }

    private async Task StartAsync(Team team, Sprint sprint)
    {
        await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);
    }

    private async Task CloseAsync(WorkItem workItem)
    {
        var tracked = await _dbContext.WorkItems
            .FirstAsync(wi => wi.Id == workItem.Id, TestContext.Current.CancellationToken);
        tracked.Close();
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
