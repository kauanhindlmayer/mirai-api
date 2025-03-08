using Application.Common.Interfaces.Persistence;
using Application.Retrospectives.Commands.CreateRetrospective;
using Domain.Retrospectives;
using Domain.Teams;

namespace Application.UnitTests.Retrospectives.Commands;

public class CreateRetrospectiveTests
{
    private static readonly CreateRetrospectiveCommand Command = new(
        "Test Retrospective",
        5,
        Domain.Retrospectives.Enums.RetrospectiveTemplate.StartStopContinue,
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
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(null as Team);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRetrospectiveAlreadyExists_ShouldReturnError()
    {
        var team = new Team(Guid.NewGuid(), "Test Team", "Test Description");
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            null,
            Command.TeamId);
        team.AddRetrospective(retrospective);
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateRetrospective()
    {
        var team = new Team(Guid.NewGuid(), "Test Team", "Test Description");
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _teamsRepository.Received().Update(team);
        team.Retrospectives.Should().Contain(r => r.Id == result.Value);
    }
}