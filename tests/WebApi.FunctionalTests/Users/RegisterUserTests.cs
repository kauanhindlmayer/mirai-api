using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
    public RegisterUserTests(FunctionalTestWebAppFactory factory)
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
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/users/register",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = UserRequestFactory.CreateRegisterUserRequest(email: "johndoe@email.com");

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/users/register",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userId = await response.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        userId.Should().NotBeEmpty();
    }
}
