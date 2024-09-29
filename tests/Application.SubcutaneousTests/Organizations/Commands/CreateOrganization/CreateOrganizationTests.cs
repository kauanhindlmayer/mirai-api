using Domain.Organizations;
using TestCommon.Organizations;

namespace Application.SubcutaneousTests.Organizations.Commands.CreateOrganization;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CreateOrganizationTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task CreateOrganization_WhenValidCommand_ShouldCreateOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();

        // Act
        var result = await _mediator.Send(createOrganizationRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(createOrganizationRequest.Name);
        result.Value.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task CreateOrganization_WhenOrganizationWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        await _mediator.Send(createOrganizationRequest);

        // Act
        var result = await _mediator.Send(createOrganizationRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(OrganizationErrors.OrganizationWithSameNameAlreadyExists);
    }
}