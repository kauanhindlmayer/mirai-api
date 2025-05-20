using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class UpdateUserProfileTests : BaseFunctionalTest
{
    public UpdateUserProfileTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateUserProfile_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var request = UserRequestFactory.CreateUpdateUserProfileRequest();

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            Routes.Users.UpdateProfile,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
