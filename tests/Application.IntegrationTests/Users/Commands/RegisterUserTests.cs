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

    private static readonly RegisterUserCommand Command = new(
        "john.doe@email.com",
        "vXJu9zCgjOV2dW3",
        "John",
        "Doe");

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsAlreadyExistsError()
    {
        // Arrange
        await _sender.Send(Command, TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().Be(UserErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUserId()
    {
        // Arrange
        var command = Command with { Email = "john.doe2@gmail.com" };

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }
}