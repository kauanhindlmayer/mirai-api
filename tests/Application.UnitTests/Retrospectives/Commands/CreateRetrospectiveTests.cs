using Application.Common.Interfaces.Persistence;
using Application.Retrospectives.Commands.CreateRetrospective;
using Domain.Retrospectives;
using Domain.Retrospectives.Enums;
using Domain.Teams;

namespace Application.UnitTests.Retrospectives.Commands;

public class CreateRetrospectiveTests
{
    private static readonly CreateRetrospectiveCommand Command = new(
        "Test Retrospective",
        5,
        RetrospectiveTemplate.StartStopContinue,
        Guid.NewGuid());

    private readonly CreateRetrospectiveCommandHandler _handler;
    private readonly ITeamsRepository _teamsRepository;

    public CreateRetrospectiveTests()
    {
        _teamsRepository = Substitute.For<ITeamsRepository>();
        _handler = new CreateRetrospectiveCommandHandler(_teamsRepository);
    }

    [Fact]
    public async Task Handle_WhenTeamDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(null as Team);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRetrospectiveAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Test Team", "Test Description");
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        team.AddRetrospective(retrospective);
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateRetrospective()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Test Team", "Test Description");
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _teamsRepository.Received().Update(team);
        team.Retrospectives.Should().Contain(r => r.Id == result.Value);
    }
}