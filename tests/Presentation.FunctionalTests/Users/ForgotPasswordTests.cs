using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class ForgotPasswordTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task ForgotPassword_WhenEmailIsRegistered_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateForgotPasswordRequest();

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.ForgotPassword,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_WhenEmailIsNotRegistered_ShouldStillReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateForgotPasswordRequest("no-such-user@mirai.com");

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.ForgotPassword,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
