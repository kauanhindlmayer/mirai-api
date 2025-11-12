using System.Net.Http.Json;
using Application.Users.Queries.LoginUser;
using FluentAssertions;
using Presentation.Controllers.Users;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class LoginUserTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task LoginUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = new LoginUserRequest(
            $"john.doe{Random.Shared.Next(1, 100)}@mirai.com",
            "password123");

        // Act
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Users.Login,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accessToken = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        accessToken.Should().NotBeNull();
        accessToken.AccessToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginUser_WhenRequestIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginUserRequest("invalid-email", "password123");

        // Act
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Users.Login,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
