using System.Net;
using System.Net.Http.Json;
using Application.Users.Queries.GetCurrentUser;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class GetLoggedInUserTests : BaseFunctionalTest
{
    public GetLoggedInUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoggedInUser_WhenRequestIsValid_ShouldReturnUser()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();

        // Act
        var response = await _httpClient.GetAsync(
            "api/users/me",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
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
        var response = await _httpClient.GetAsync(
            "api/users/me",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
