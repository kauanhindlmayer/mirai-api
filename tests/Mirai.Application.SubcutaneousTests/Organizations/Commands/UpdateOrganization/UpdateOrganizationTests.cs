using Mirai.Application.Organizations.Commands.UpdateOrganization;
using Mirai.Domain.Organizations;
using TestCommon.Organizations;

namespace Mirai.Application.SubcutaneousTests.Organizations.Commands.UpdateOrganization;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateOrganizationTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task UpdateOrganization_WhenValidCommand_ShouldUpdateOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var createOrganizationResult = await _mediator.Send(createOrganizationRequest);

        // Act
        var updateOrganizationRequest = new UpdateOrganizationCommand(createOrganizationResult.Value.Id, "New Name", "New Description");
        var result = await _mediator.Send(updateOrganizationRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("New Name");
        result.Value.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var updateOrganizationRequest = new UpdateOrganizationCommand(Guid.NewGuid(), "New Name", "New Description");

        // Act
        var result = await _mediator.Send(updateOrganizationRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(OrganizationErrors.OrganizationNotFound);
    }
}