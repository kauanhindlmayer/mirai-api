using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class ResetPasswordTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task ResetPassword_WhenTokenIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = UserRequestFactory.CreateResetPasswordRequest();

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.ResetPassword,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
