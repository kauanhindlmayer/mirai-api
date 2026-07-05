using Application.IntegrationTests.Infrastructure;
using Application.Users.Commands.LoginWithGitHub;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Commands;

public class LoginWithGitHubTests : BaseIntegrationTest
{
    public LoginWithGitHubTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenAuthorizationCodeIsInvalid_ShouldReturnError()
    {
        // Arrange
        var command = new LoginWithGitHubCommand(
            "invalid-code",
            "https://localhost:5173/auth/github/callback");

        // Act
        var result = await _sender.Send(
            command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidCredentials);
    }
}
