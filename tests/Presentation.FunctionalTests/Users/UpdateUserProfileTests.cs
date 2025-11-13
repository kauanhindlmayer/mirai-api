using System.Net.Http.Json;
using FluentAssertions;
using Presentation.Controllers.Users;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class UpdateUserProfileTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task UpdateUserProfile_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = new UpdateUserProfileRequest("Jane", "Smith");

        // Act
        var response = await fixture.HttpClient.PutAsJsonAsync(
            Routes.Users.UpdateProfile,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
