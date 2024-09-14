using TestCommon.Organizations;

namespace Mirai.Application.SubcutaneousTests.Organizations.Commands.CreateOrganization;

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
}