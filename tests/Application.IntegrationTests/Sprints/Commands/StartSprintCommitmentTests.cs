using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.StartSprint;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Sprints.Commands;

public class StartSprintCommitmentTests : BaseIntegrationTest
{
    public StartSprintCommitmentTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task StartSprint_ShouldPersistOneCommitmentPerWorkItem()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);
        var estimated = await SeedWorkItemInSprintAsync(team, sprint, code: 1, storyPoints: 5);
        await SeedWorkItemInSprintAsync(team, sprint, code: 2, storyPoints: null);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var commitments = await _dbContext
            .Set<SprintCommitment>()
            .AsNoTracking()
            .Where(c => c.SprintId == sprint.Id)
            .ToListAsync(TestContext.Current.CancellationToken);

        commitments.Should().HaveCount(2);
        commitments.Should().ContainSingle(c =>
            c.WorkItemId == estimated.Id && c.CommittedStoryPoints == 5);
        commitments.Should().ContainSingle(c => c.CommittedStoryPoints == null);
    }

    [Fact]
    public async Task StartSprint_WhenSprintIsEmpty_ShouldRecordNoCommitments()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var commitmentCount = await _dbContext
            .Set<SprintCommitment>()
            .CountAsync(c => c.SprintId == sprint.Id, TestContext.Current.CancellationToken);
        commitmentCount.Should().Be(0);
    }

    [Fact]
    public async Task StartSprint_ShouldFreezeCommittedPointsAgainstLaterReEstimation()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);
        var workItem = await SeedWorkItemInSprintAsync(team, sprint, code: 1, storyPoints: 5);

        await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act - re-estimate the work item after the sprint has started
        var tracked = await _dbContext.WorkItems
            .FirstAsync(wi => wi.Id == workItem.Id, TestContext.Current.CancellationToken);
        tracked.Update(planning: new Planning { StoryPoints = 13 });
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert - the commitment still holds the points as they were at start
        var commitment = await _dbContext
            .Set<SprintCommitment>()
            .AsNoTracking()
            .FirstAsync(c => c.WorkItemId == workItem.Id, TestContext.Current.CancellationToken);
        commitment.CommittedStoryPoints.Should().Be(5);
    }

    private async Task<Sprint> SeedSprintAsync(Team team)
    {
        var startDate = new DateOnly(2026, 6, 1);
        var sprint = new Sprint(team.Id, "Sprint 1", startDate, startDate.AddDays(13));
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
}
