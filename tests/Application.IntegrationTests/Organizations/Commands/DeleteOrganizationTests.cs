using Application.IntegrationTests.Common;
using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Domain.Organizations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Organizations.Commands;

public class DeleteOrganizationTests : BaseIntegrationTest
{
    public DeleteOrganizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var command = new DeleteOrganizationCommand(Guid.NewGuid());

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task DeleteOrganization_WhenValidCommand_ShouldDeleteOrganization()
    {
        // Arrange
        var createCommand = new CreateOrganizationCommand("Test Organization", "Test Description");
        var createOrganizationResult = await _sender.Send(createCommand);
        var organizationId = createOrganizationResult.Value;

        // Act
        var deleteCommand = new DeleteOrganizationCommand(organizationId);
        var result = await _sender.Send(deleteCommand);

        // Assert
        result.IsError.Should().BeFalse();
        _dbContext.Organizations
            .AsNoTracking()
            .Should().NotContain(x => x.Id == organizationId);
    }
}