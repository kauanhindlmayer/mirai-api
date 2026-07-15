using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.UpdateSprint;
using Domain.Authorization;
using Domain.Sprints;
using Domain.Teams;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Sprints.Commands;

public class UpdateSprintTests : BaseIntegrationTest
{
    public UpdateSprintTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var startDate = new DateOnly(2026, 6, 1);
        var command = new UpdateSprintCommand(
            team.Id,
            Guid.NewGuid(),
            "Sprint 1",
            startDate,
            startDate.AddDays(13));

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotFound);
    }

    [Fact]
    public async Task UpdateSprint_ShouldPersistTheNewNameAndDates()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));

        var command = new UpdateSprintCommand(
            team.Id,
            sprint.Id,
            "Sprint 42",
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 14));

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var updatedSprint = await _dbContext.Sprints
            .AsNoTracking()
            .FirstAsync(s => s.Id == sprint.Id, TestContext.Current.CancellationToken);
        updatedSprint.Name.Should().Be("Sprint 42");
        updatedSprint.StartDate.Should().Be(new DateOnly(2026, 7, 1));
        updatedSprint.EndDate.Should().Be(new DateOnly(2026, 7, 14));
    }

    [Fact]
    public async Task UpdateSprint_WhenDatesOverlapAnotherSprint_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var sprint = await SeedSprintAsync(team, "Sprint 2", new DateOnly(2026, 6, 15));

        var command = new UpdateSprintCommand(
            team.Id,
            sprint.Id,
            "Sprint 2",
            new DateOnly(2026, 6, 10),
            new DateOnly(2026, 6, 28));

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.Overlaps.Code);
    }

    [Fact]
    public async Task UpdateSprint_WhenNameIsTakenByAnotherSprint_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var sprint = await SeedSprintAsync(team, "Sprint 2", new DateOnly(2026, 6, 15));

        var command = new UpdateSprintCommand(
            team.Id,
            sprint.Id,
            "Sprint 1",
            sprint.StartDate,
            sprint.EndDate);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.AlreadyExists.Code);
    }

    private async Task<Sprint> SeedSprintAsync(Team team, string name, DateOnly startDate)
    {
        var sprint = new Sprint(team.Id, name, startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }
}
