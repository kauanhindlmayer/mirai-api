using Application.IntegrationTests.Common;
using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;
using FluentAssertions;

namespace Application.IntegrationTests.Organizations.Commands;

public class CreateOrganizationTests : BaseIntegrationTest
{
    public CreateOrganizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrganization_WhenValidCommand_ShouldCreateOrganization()
    {
        // Arrange
        var command = new CreateOrganizationCommand("Test Organization", "Test Description");

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateOrganization_WhenOrganizationExists_ShouldReturnError()
    {
        // Arrange
        var command = new CreateOrganizationCommand("Test Organization 2", "Test Description 2");
        await _sender.Send(command, TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().Be(OrganizationErrors.AlreadyExists);
    }
}