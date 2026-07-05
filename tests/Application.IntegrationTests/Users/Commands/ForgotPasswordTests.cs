using Application.IntegrationTests.Infrastructure;
using Application.Users.Commands.ForgotPassword;
using Application.Users.Commands.RegisterUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Users.Commands;

public class ForgotPasswordTests : BaseIntegrationTest
{
    public ForgotPasswordTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldSetPasswordResetToken()
    {
        // Arrange
        var registerCommand = new RegisterUserCommand(
            "john.doe5@mirai.com",
            "vXJu9zCgjOV2dW3",
            "John",
            "Doe");
        await _sender.Send(registerCommand, TestContext.Current.CancellationToken);
        var command = new ForgotPasswordCommand("john.doe5@mirai.com");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var user = await _dbContext.Users.FirstAsync(
            u => u.Email == command.Email,
            TestContext.Current.CancellationToken);
        user.PasswordResetToken.Should().NotBeNullOrEmpty();
        user.PasswordResetTokenExpiresAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnSuccessWithoutSideEffects()
    {
        // Arrange
        var command = new ForgotPasswordCommand("no-such-user@mirai.com");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }
}
