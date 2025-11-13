using System.Net.Http.Json;
using FluentAssertions;
using Presentation.Controllers.Users;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Users;

public class RegisterUserTests(DistributedApplicationTestFixture fixture)
{
    [Theory]
    [ClassData(typeof(InvalidRegisterUserRequestData))]
    public async Task RegisterUser_WhenRequestIsInvalid_ShouldReturnBadRequest(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        // Arrange
        var request = new RegisterUserRequest(email, password, firstName, lastName);

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.Register,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = new RegisterUserRequest(
            $"user-{Guid.NewGuid()}@mirai.com",
            UserRequestFactory.Password,
            "John",
            "Doe");

        // Act
        var response = await fixture.UnauthenticatedHttpClient.PostAsJsonAsync(
            Routes.Users.Register,
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userId = await response.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        userId.Should().NotBeEmpty();
    }
}
