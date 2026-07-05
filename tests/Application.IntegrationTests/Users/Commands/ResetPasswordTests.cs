using Application.IntegrationTests.Infrastructure;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.ResetPassword;
using Application.Users.Queries.LoginUser;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Users.Commands;

public class ResetPasswordTests : BaseIntegrationTest
{
    public ResetPasswordTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenTokenIsValid_ShouldResetPasswordAndAllowLoginWithNewPassword()
    {
        // Arrange
        const string email = "john.doe6@mirai.com";
        const string oldPassword = "vXJu9zCgjOV2dW3";
        const string newPassword = "n3wP@ssw0rd123";
        var registerCommand = new RegisterUserCommand(email, oldPassword, "John", "Doe");
        await _sender.Send(registerCommand, TestContext.Current.CancellationToken);
        var user = await _dbContext.Users.FirstAsync(
            u => u.Email == email,
            TestContext.Current.CancellationToken);
        user.SetPasswordResetToken("integration-test-token", DateTime.UtcNow.AddHours(1));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var command = new ResetPasswordCommand(email, "integration-test-token", newPassword);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var updatedUser = await _dbContext.Users.FirstAsync(
            u => u.Email == email,
            TestContext.Current.CancellationToken);
        updatedUser.PasswordResetToken.Should().BeNull();

        var loginWithNewPassword = await _sender.Send(
            new LoginUserQuery(email, newPassword),
            TestContext.Current.CancellationToken);
        loginWithNewPassword.IsError.Should().BeFalse();

        var loginWithOldPassword = await _sender.Send(
            new LoginUserQuery(email, oldPassword),
            TestContext.Current.CancellationToken);
        loginWithOldPassword.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenTokenIsExpired_ShouldReturnError()
    {
        // Arrange
        const string email = "john.doe7@mirai.com";
        var registerCommand = new RegisterUserCommand(email, "vXJu9zCgjOV2dW3", "John", "Doe");
        await _sender.Send(registerCommand, TestContext.Current.CancellationToken);
        var user = await _dbContext.Users.FirstAsync(
            u => u.Email == email,
            TestContext.Current.CancellationToken);
        user.SetPasswordResetToken("expired-token", DateTime.UtcNow.AddHours(-1));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var command = new ResetPasswordCommand(email, "expired-token", "n3wP@ssw0rd123");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidOrExpiredPasswordResetToken);
    }

    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            "no-such-user@mirai.com",
            "garbage-token",
            "n3wP@ssw0rd123");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidOrExpiredPasswordResetToken);
    }
}
