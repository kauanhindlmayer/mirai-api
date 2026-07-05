using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class LoginWithGitHubTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task LoginWithGitHub_WhenAuthorizationCodeIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = UserRequestFactory.CreateLoginWithGitHubRequest();

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.LoginWithGitHub,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
