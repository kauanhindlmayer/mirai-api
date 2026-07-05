using Application.Abstractions.Authentication;
using Application.Users.Commands.ResetPassword;
using Domain.Users;

namespace Application.UnitTests.Users.Commands;

public class ResetPasswordTests
{
    private static readonly ResetPasswordCommand Command = new(
        "john.doe@mirai.com",
        "valid-token",
        "NewPassword@123");

    private readonly ResetPasswordCommandHandler _handler;
    private readonly IUserRepository _mockUserRepository;
    private readonly IAuthenticationService _mockAuthenticationService;

    public ResetPasswordTests()
    {
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockAuthenticationService = Substitute.For<IAuthenticationService>();
        _handler = new ResetPasswordCommandHandler(
            _mockUserRepository,
            _mockAuthenticationService);
    }

    [Fact]
    public async Task Handle_WhenTokenIsValid_ResetsPasswordAndClearsToken()
    {
        // Arrange
        var user = new User("John", "Doe", Command.Email);
        user.SetIdentityId(Guid.NewGuid().ToString());
        user.SetPasswordResetToken(Command.Token, DateTime.UtcNow.AddHours(1));
        _mockUserRepository.GetByPasswordResetTokenAsync(
            Command.Token,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        user.PasswordResetToken.Should().BeNull();
        user.PasswordResetTokenExpiresAtUtc.Should().BeNull();
        await _mockAuthenticationService.Received(1).ResetPasswordAsync(
            user.IdentityId,
            Command.NewPassword,
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Handle_WhenTokenIsExpired_ReturnsInvalidOrExpiredTokenError()
    {
        // Arrange
        var user = new User("John", "Doe", Command.Email);
        user.SetPasswordResetToken(Command.Token, DateTime.UtcNow.AddHours(-1));
        _mockUserRepository.GetByPasswordResetTokenAsync(
            Command.Token,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidOrExpiredPasswordResetToken);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotMatchToken_ReturnsInvalidOrExpiredTokenError()
    {
        // Arrange
        var user = new User("John", "Doe", "someone.else@mirai.com");
        user.SetPasswordResetToken(Command.Token, DateTime.UtcNow.AddHours(1));
        _mockUserRepository.GetByPasswordResetTokenAsync(
            Command.Token,
            TestContext.Current.CancellationToken)
            .Returns(user);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidOrExpiredPasswordResetToken);
    }

    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_ReturnsInvalidOrExpiredTokenError()
    {
        // Arrange
        _mockUserRepository.GetByPasswordResetTokenAsync(
            Command.Token,
            TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidOrExpiredPasswordResetToken);
    }
}
