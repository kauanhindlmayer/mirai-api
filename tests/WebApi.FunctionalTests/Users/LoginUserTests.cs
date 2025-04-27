using System.Net;
using System.Net.Http.Json;
using Application.Users.Common;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class LoginUserTests : BaseFunctionalTest
{
    public LoginUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task LoginUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateLoginUserRequest();

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/users/login",
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
        var request = UserRequestFactory.CreateLoginUserRequest("invalid-email");

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/users/login",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
