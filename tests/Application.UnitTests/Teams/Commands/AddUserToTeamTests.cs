using Application.Teams.Commands.AddUserToTeam;
using Domain.Teams;
using Domain.Users;

namespace Application.UnitTests.Teams.Commands;

public class AddUserToTeamTests
{
    private static readonly AddUserToTeamCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly AddUserToTeamCommandHandler _handler;
    private readonly ITeamsRepository _teamsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddUserToTeamTests()
    {
        _teamsRepository = Substitute.For<ITeamsRepository>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _handler = new AddUserToTeamCommandHandler(
            _teamsRepository,
            _usersRepository);
    }

    [Fact]
    public async Task Handle_WhenTeamDoesNotExist_ShouldReturnError()
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
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        _teamsRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.UserId, TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var user = new User("John", "Doe", "john.doe@email.com");
        team.AddUser(user);
        _teamsRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.UserId, TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.UserAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldAddUserToTeam()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var user = new User("John", "Doe", "john.doe@email.com");
        _teamsRepository.GetByIdAsync(Command.TeamId, TestContext.Current.CancellationToken)
            .Returns(team);
        _usersRepository.GetByIdAsync(Command.UserId, TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        team.Users.Should().Contain(user);
        _teamsRepository.Received().Update(team);
    }
}