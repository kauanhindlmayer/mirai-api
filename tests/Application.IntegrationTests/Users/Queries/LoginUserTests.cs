using Application.IntegrationTests.Common;
using Application.Users.Commands.RegisterUser;
using Application.Users.Common;
using Application.Users.Queries.LoginUser;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Queries;

public class LoginUserTests : BaseIntegrationTest
{
    public LoginUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new LoginUserQuery("test@test.com", "password");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordMatches_ReturnsValidAccessTokenResponse()
    {
        // Arrange
        var registerCommand = new RegisterUserCommand(
            "john.doe3@email.com",
            "vXJu9zCgjOV2dW3",
            "John",
            "Doe");
        await _sender.Send(registerCommand, TestContext.Current.CancellationToken);
        var command = new LoginUserQuery("john.doe3@email.com", "vXJu9zCgjOV2dW3");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<AccessTokenResponse>();
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordDoesNotMatch_ReturnsAuthenticationFailedError()
    {
        var registerCommand = new RegisterUserCommand(
            "john.doe4@email.com",
            "vXJu9zCgjOV2dW3",
            "John",
            "Doe");
        await _sender.Send(registerCommand, TestContext.Current.CancellationToken);
        var command = new LoginUserQuery("john.doe4@email.com", "wrong_password");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidCredentials);
    }
}