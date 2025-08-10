using Application.Sprints.Commands.CreateSprint;
using Domain.Sprints;
using Domain.Teams;

namespace Application.UnitTests.Sprints.Commands;

public class CreateSprintTests
{
    private static readonly CreateSprintCommand Command = new(
        Guid.NewGuid(),
        "Name",
        DateOnly.FromDateTime(DateTime.UtcNow),
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));

    private readonly CreateSprintCommandHandler _handler;
    private readonly ITeamRepository _teamRepository;

    public CreateSprintTests()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _handler = new CreateSprintCommandHandler(_teamRepository);
    }

    [Fact]
    public async Task Handle_WhenTeamDoesNotExists_ShouldReturnError()
    {
        // Arrange
        _teamRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
            .Returns(null as Team);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSprintAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Team", string.Empty);
        var sprint = new Sprint(
            team.Id,
            "Sprint 1",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));
        team.AddSprint(sprint);
        _teamRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
            .Returns(team);

        // Act
        var result = await _handler.Handle(
            Command with { Name = sprint.Name },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.AlreadyExists);
    }
}