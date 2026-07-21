using Application.IntegrationTests.Infrastructure;
using Application.Sprints.Commands.StartSprint;
using Domain.Sprints;
using Domain.Teams;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.IntegrationTests.Sprints.Commands;

public class StartSprintTests : BaseIntegrationTest
{
    public StartSprintTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task StartSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var command = new StartSprintCommand(team.Id, Guid.NewGuid());

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotFound);
    }

    [Fact]
    public async Task StartSprint_ShouldPersistTheSprintAsActive()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var command = new StartSprintCommand(team.Id, sprint.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var startedSprint = await _dbContext.Sprints
            .AsNoTracking()
            .FirstAsync(s => s.Id == sprint.Id, TestContext.Current.CancellationToken);
        startedSprint.Status.Should().Be(SprintStatus.Active);
        startedSprint.StartedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task StartSprint_WhenAnotherSprintIsActive_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var active = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var next = await SeedSprintAsync(team, "Sprint 2", new DateOnly(2026, 6, 15));

        await _sender.Send(
            new StartSprintCommand(team.Id, active.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new StartSprintCommand(team.Id, next.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.TeamAlreadyHasActiveSprint("any").Code);
    }

    /// <summary>
    /// The domain guard cannot see a concurrent starter, so the one-active-per-team
    /// rule ultimately rests on a filtered unique index. Writing a second active
    /// sprint straight past the domain proves the database refuses it.
    /// </summary>
    [Fact]
    public async Task StartSprint_WhenASecondSprintIsForcedActive_ShouldBeRejectedByTheDatabase()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var first = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));
        var second = await SeedSprintAsync(team, "Sprint 2", new DateOnly(2026, 6, 15));

        await _sender.Send(
            new StartSprintCommand(team.Id, first.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        var forceSecondActive = async () =>
        {
            var tracked = await _dbContext.Sprints
                .FirstAsync(s => s.Id == second.Id, TestContext.Current.CancellationToken);
            tracked.Start();
            await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        };

        // Assert
        var exception = await forceSecondActive.Should().ThrowAsync<DbUpdateException>();
        exception.WithInnerException<PostgresException>()
            .Which.SqlState.Should().Be(PostgresErrorCodes.UniqueViolation);
    }

    [Fact]
    public async Task StartSprint_WhenSprintIsAlreadyActive_ShouldReturnError()
    {
        // Arrange
        var team = await SeedTeamWithAdminAsync();
        var sprint = await SeedSprintAsync(team, "Sprint 1", new DateOnly(2026, 6, 1));

        await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new StartSprintCommand(team.Id, sprint.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.NotPlanned.Code);
    }

    [Fact]
    public async Task StartSprint_WhenTeamsDifferent_ShouldNotBlockEachOther()
    {
        // Arrange
        var firstTeam = await SeedTeamWithAdminAsync();
        var firstSprint = await SeedSprintAsync(firstTeam, "Sprint 1", new DateOnly(2026, 6, 1));
        await _sender.Send(
            new StartSprintCommand(firstTeam.Id, firstSprint.Id),
            TestContext.Current.CancellationToken);

        var secondTeam = await SeedTeamWithAdminAsync();
        var secondSprint = await SeedSprintAsync(secondTeam, "Sprint 1", new DateOnly(2026, 6, 1));

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(
            new StartSprintCommand(secondTeam.Id, secondSprint.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    private async Task<Sprint> SeedSprintAsync(Team team, string name, DateOnly startDate)
    {
        var sprint = new Sprint(team.Id, name, startDate, startDate.AddDays(13));
        team.AddSprint(sprint);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return sprint;
    }
}
