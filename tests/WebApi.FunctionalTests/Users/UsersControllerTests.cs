using System.Net;
using System.Net.Http.Json;
using Application.Users.Common;
using Contracts.Users;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class UserControllerTests : BaseFunctionalTest
{
    public UserControllerTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Theory]
    [ClassData(typeof(InvalidRegisterUserRequestData))]
    public async Task RegisterUser_WhenRequestIsInvalid_ShouldReturnBadRequest(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        // Arrange
        var request = UserRequestFactory.CreateRegisterUserRequest(
            email,
            password,
            firstName,
            lastName);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateRegisterUserRequest();

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userId = await response.Content.ReadFromJsonAsync<Guid>();
        userId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateLoginUserRequest();

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accessToken = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        accessToken!.AccessToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginUser_WhenRequestIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = UserRequestFactory.CreateLoginUserRequest("invalid-email");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenRequestIsValid_ShouldReturnUser()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();

        // Act
        var response = await HttpClient.GetAsync("api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user!.Id.Should().NotBeEmpty();
        user.Email.Should().Be(UserRequestFactory.Email);
        user.FirstName.Should().Be(UserRequestFactory.FirstName);
        user.LastName.Should().Be(UserRequestFactory.LastName);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenAccessTokenIsMissing_ShouldReturnUnauthorized()
    {
        // Act
        var response = await HttpClient.GetAsync("api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
