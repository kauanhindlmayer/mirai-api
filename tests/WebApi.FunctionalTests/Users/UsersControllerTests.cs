using System.Net;
using System.Net.Http.Json;
using Application.Users.Common;
using Application.Users.Queries.GetCurrentUser;
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
        var response = await _httpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateRegisterUserRequest(email: "johndoe@email.com");

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/v1/users/register", request);

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
        var response = await _httpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accessToken = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        accessToken.Should().NotBeNull();
        accessToken.AccessToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginUser_WhenRequestIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = UserRequestFactory.CreateLoginUserRequest("invalid-email");

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenRequestIsValid_ShouldReturnUser()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();

        // Act
        var response = await _httpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(UserRequestFactory.Email);
        user.FirstName.Should().Be(UserRequestFactory.FirstName);
        user.LastName.Should().Be(UserRequestFactory.LastName);
    }

    [Fact]
    public async Task GetLoggedInUser_WhenAccessTokenIsMissing_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _httpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUserProfile_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var request = UserRequestFactory.CreateUpdateUserProfileRequest();

        // Act
        var response = await _httpClient.PutAsJsonAsync("api/v1/users/profile", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
