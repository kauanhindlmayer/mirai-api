using System.Net.Http.Json;
using Application.Users.Queries.GetCurrentUser;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class GetLoggedInUserTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task GetLoggedInUser_WhenRequestIsValid_ShouldReturnUser()
    {
        // Act
        var response = await fixture.HttpClient.GetAsync(
            Routes.Users.GetLoggedInUser,
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
        var response = await fixture.HttpClient.GetAsync(
            Routes.Users.GetLoggedInUser,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
