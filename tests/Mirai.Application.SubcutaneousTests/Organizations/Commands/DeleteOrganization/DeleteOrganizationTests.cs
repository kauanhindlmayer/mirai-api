using ErrorOr;
using Mirai.Application.Organizations.Commands.DeleteOrganization;
using Mirai.Domain.Organizations;
using TestCommon.Organizations;

namespace Mirai.Application.SubcutaneousTests.Organizations.Commands.DeleteOrganization;

[Collection(WebAppFactoryCollection.CollectionName)]
public class DeleteOrganizationTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task DeleteOrganization_WhenValidCommand_ShouldDeleteOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var createOrganizationResult = await _mediator.Send(createOrganizationRequest);

        // Act
        var deleteOrganizationRequest = new DeleteOrganizationCommand(createOrganizationResult.Value.Id);
        var result = await _mediator.Send(deleteOrganizationRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(Result.Success);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var deleteOrganizationRequest = new DeleteOrganizationCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(deleteOrganizationRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(OrganizationErrors.OrganizationNotFound);
    }
}