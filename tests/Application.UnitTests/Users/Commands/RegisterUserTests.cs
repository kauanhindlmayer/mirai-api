using Application.Abstractions.Authentication;
using Application.Users.Commands.RegisterUser;
using Domain.Users;

namespace Application.UnitTests.Users.Commands;

public class RegisterUserTests
{
    private static readonly RegisterUserCommand Command = new(
        "john.doe@mirai.com",
        "password",
        "John",
        "Doe");

    private readonly RegisterUserCommandHandler _handler;
    private readonly IUserRepository _mockUserRepository;
    private readonly IAuthenticationService _mockAuthenticationService;

    public RegisterUserTests()
    {
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockAuthenticationService = Substitute.For<IAuthenticationService>();
        _handler = new RegisterUserCommandHandler(
            _mockUserRepository,
            _mockAuthenticationService);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsAlreadyExistsError()
    {
        // Arrange
        _mockUserRepository.ExistsByEmailAsync(
            Command.Email,
            TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUserId()
    {
        // Arrange
        _mockUserRepository.ExistsByEmailAsync(
            Command.Email,
            TestContext.Current.CancellationToken)
            .Returns(false);
        _mockAuthenticationService.RegisterAsync(
            Arg.Any<User>(),
            Arg.Any<string>(),
            TestContext.Current.CancellationToken)
            .Returns(Guid.NewGuid().ToString());

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }
}