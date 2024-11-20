using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contracts.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class UserControllerTests : BaseFunctionalTest
{
    public UserControllerTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task RegisterUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateRegisterUserRequest(email: "new.user@gmail.com");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenAccessTokenIsMissing_ShouldReturnUnauthorized()
    {
        // Act
        var response = await HttpClient.GetAsync("api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenRequestIsValid_ShouldReturnUser()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        var user = await HttpClient.GetFromJsonAsync<UserResponse>("api/users/me");

        // Assert
        user.Should().NotBeNull();
    }
}
