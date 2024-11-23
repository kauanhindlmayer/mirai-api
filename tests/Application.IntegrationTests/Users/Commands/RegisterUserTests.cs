using Application.IntegrationTests.Common;
using Application.Users.Commands.RegisterUser;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Users.Commands;

public class RegisterUserTests : BaseIntegrationTest
{
    public RegisterUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsAlreadyExistsError()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "john.doe@email.com",
            "password",
            "John",
            "Doe");
        await Sender.Send(command);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().Be(UserErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUserId()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "john.doe@email.com",
            "password",
            "John",
            "Doe");

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
    }
}