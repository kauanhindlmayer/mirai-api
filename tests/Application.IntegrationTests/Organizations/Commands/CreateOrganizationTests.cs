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
        var result = await _sender.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Organization>();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task CreateOrganization_WhenOrganizationExists_ShouldReturnError()
    {
        // Arrange
        var command = new CreateOrganizationCommand("Test Organization 2", "Test Description 2");
        await _sender.Send(command);

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().Be(OrganizationErrors.AlreadyExists);
    }
}