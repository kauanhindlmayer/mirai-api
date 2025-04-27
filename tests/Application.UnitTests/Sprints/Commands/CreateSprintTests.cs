using Application.Common.Interfaces.Persistence;
using Application.Sprints.Commands.CreateSprint;
using Domain.Sprints;
using Domain.Teams;

namespace Application.UnitTests.Sprints.Commands;

public class CreateSprintTests
{
    private static readonly CreateSprintCommand Command = new(
        Guid.NewGuid(),
        "Name",
        DateTime.UtcNow,
        DateTime.UtcNow.AddDays(14));

    private readonly CreateSprintCommandHandler _handler;
    private readonly ITeamsRepository _teamsRepository;

    public CreateSprintTests()
    {
        _teamsRepository = Substitute.For<ITeamsRepository>();
        _handler = new CreateSprintCommandHandler(_teamsRepository);
    }

    [Fact]
    public async Task Handle_WhenTeamDoesNotExists_ShouldReturnError()
    {
        // Arrange
        _teamsRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
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
        var sprint = new Sprint(team.Id, "Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        team.AddSprint(sprint);
        _teamsRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
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