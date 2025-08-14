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
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public AddUserToTeamTests()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new AddUserToTeamCommandHandler(
            _teamRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_WhenTeamDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _teamRepository.GetByIdAsync(
            Command.TeamId,
            TestContext.Current.CancellationToken)
            .Returns(null as Team);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        _teamRepository.GetByIdAsync(
            Command.TeamId,
            TestContext.Current.CancellationToken)
            .Returns(team);
        _userRepository.GetByIdAsync(
            Command.UserId,
            TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var user = new User("John", "Doe", "john.doe@mirai.com");
        team.AddUser(user);
        _teamRepository.GetByIdAsync(
            Command.TeamId,
            TestContext.Current.CancellationToken)
            .Returns(team);
        _userRepository.GetByIdAsync(
            Command.UserId,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TeamErrors.UserAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldAddUserToTeam()
    {
        // Arrange
        var team = new Team(Guid.NewGuid(), "Name", "Description");
        var user = new User("John", "Doe", "john.doe@mirai.com");
        _teamRepository.GetByIdAsync(
            Command.TeamId,
            TestContext.Current.CancellationToken)
            .Returns(team);
        _userRepository.GetByIdAsync(
            Command.UserId,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        team.Users.Should().Contain(user);
        _teamRepository.Received().Update(team);
    }
}