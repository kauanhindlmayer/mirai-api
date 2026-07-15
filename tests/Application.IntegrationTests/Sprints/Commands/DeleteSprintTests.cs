using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.DeleteSprint;
using Domain.Authorization;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Sprints.Commands;

public class DeleteSprintTests : BaseIntegrationTest
{
    public DeleteSprintTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var command = new DeleteSprintCommand(team.Id, Guid.NewGuid());

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotFound);
    }

    [Fact]
    public async Task DeleteSprint_WhenSprintIsEmpty_ShouldDeleteSprint()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);
        var command = new DeleteSprintCommand(team.Id, sprint.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var deletedSprint = await _dbContext.Sprints
            .FirstOrDefaultAsync(s => s.Id == sprint.Id, TestContext.Current.CancellationToken);
        deletedSprint.Should().BeNull();
    }

    [Fact]
    public async Task DeleteSprint_WhenSprintHasWorkItems_ShouldReturnThemToTheBacklog()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team);

        var workItem = new WorkItem(
            team.ProjectId,
            code: 1,
            title: "Carry me back to the backlog",
            WorkItemType.UserStory,
            assignedTeamId: team.Id,
            sprintId: sprint.Id);
        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteSprintCommand(team.Id, sprint.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var deletedSprint = await _dbContext.Sprints
            .FirstOrDefaultAsync(s => s.Id == sprint.Id, TestContext.Current.CancellationToken);
        deletedSprint.Should().BeNull();

        var detachedWorkItem = await _dbContext.WorkItems
            .FirstAsync(wi => wi.Id == workItem.Id, TestContext.Current.CancellationToken);
        detachedWorkItem.SprintId.Should().BeNull();
    }

    private async Task<Sprint> SeedSprintAsync(Team team)
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var sprint = new Sprint(team.Id, "Sprint 1", startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }
}
