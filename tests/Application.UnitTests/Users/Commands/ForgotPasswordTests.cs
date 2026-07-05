using Application.Abstractions.Email;
using Application.Abstractions.Links;
using Application.Users.Commands.ForgotPassword;
using Domain.Users;

namespace Application.UnitTests.Users.Commands;

public class ForgotPasswordTests
{
    private static readonly ForgotPasswordCommand Command = new("john.doe@mirai.com");

    private readonly ForgotPasswordCommandHandler _handler;
    private readonly IUserRepository _mockUserRepository;
    private readonly IEmailService _mockEmailService;
    private readonly IFrontendLinkService _mockFrontendLinkService;

    public ForgotPasswordTests()
    {
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockEmailService = Substitute.For<IEmailService>();
        _mockFrontendLinkService = Substitute.For<IFrontendLinkService>();
        _handler = new ForgotPasswordCommandHandler(
            _mockUserRepository,
            _mockEmailService,
            _mockFrontendLinkService);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsSuccessWithoutSendingEmail()
    {
        // Arrange
        _mockUserRepository.GetByEmailAsync(
            Command.Email,
            TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockEmailService.DidNotReceive().SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExists_SetsTokenAndSendsEmail()
    {
        // Arrange
        var user = new User("John", "Doe", Command.Email);
        _mockUserRepository.GetByEmailAsync(
            Command.Email,
            TestContext.Current.CancellationToken)
            .Returns(user);
        _mockFrontendLinkService.BuildResetPasswordLink(
            Command.Email,
            Arg.Any<string>())
            .Returns("https://localhost:5173/reset-password?token=abc&email=john.doe@mirai.com");

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        user.PasswordResetToken.Should().NotBeNullOrEmpty();
        user.PasswordResetTokenExpiresAtUtc.Should().NotBeNull();
        await _mockEmailService.Received(1).SendEmailAsync(
            Command.Email,
            Arg.Any<string>(),
            Arg.Any<string>(),
            TestContext.Current.CancellationToken);
    }
}
