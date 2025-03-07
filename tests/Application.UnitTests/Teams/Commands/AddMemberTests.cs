using Application.Common.Interfaces.Persistence;
using Application.Teams.Commands.AddMember;
using Domain.Teams;
using Domain.Users;

namespace Application.UnitTests.Teams.Commands;

public class AddMemberTests
{
    private static readonly AddMemberCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly AddMemberCommandHandler _handler;
    private readonly ITeamsRepository _teamsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddMemberTests()
    {
        _teamsRepository = Substitute.For<ITeamsRepository>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _handler = new AddMemberCommandHandler(
            _teamsRepository,
            _usersRepository);
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
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenMemberDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.MemberId, Arg.Any<CancellationToken>())
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.MemberNotFound);
    }

    [Fact]
    public async Task Handle_WhenMemberAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var member = new User("John", "Doe", "john.doe@email.com");
        team.AddMember(member);
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.MemberId, Arg.Any<CancellationToken>())
            .Returns(member);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.MemberAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenMemberDoesNotExist_ShouldAddMember()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var member = new User("John", "Doe", "john.doe@email.com");
        _teamsRepository.GetByIdAsync(Command.TeamId, Arg.Any<CancellationToken>())
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.MemberId, Arg.Any<CancellationToken>())
            .Returns(member);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        team.Members.Should().Contain(member);
        _teamsRepository.Received().Update(team);
    }
}