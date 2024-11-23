using Application.IntegrationTests.Common;
using Application.Users.Commands.RegisterUser;
using Application.Users.Common;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.LoginUser;
using Contracts.Users;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Queries;

public class GetCurrentUserTests : BaseIntegrationTest
{
    public GetCurrentUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_ReturnsUser()
    {
        // Arrange
        var registerUserCommand = new RegisterUserCommand(
            "john.doe@email.com",
            "password",
            "John",
            "Doe");
        await _sender.Send(registerUserCommand);
        var loginUserCommand = new LoginUserQuery(
            "john.doe@email.com",
            "password");
        await _sender.Send(loginUserCommand);

        var query = new GetCurrentUserQuery();

        // Act
        var result = await _sender.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<UserResponse>();
        result.Value.Email.Should().Be("john.doe@email.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ReturnsUnauthorizedError()
    {
        // Arrange
        var query = new GetCurrentUserQuery();

        // Act
        var result = await _sender.Send(query);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().Be(UserErrors.AuthenticationFailed);
    }
}