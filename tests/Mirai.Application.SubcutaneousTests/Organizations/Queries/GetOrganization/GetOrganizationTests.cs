using Mirai.Application.Organizations.Queries.GetOrganization;
using Mirai.Domain.Organizations;
using TestCommon.Organizations;

namespace Mirai.Application.SubcutaneousTests.Organizations.Queries.GetOrganization;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetOrganizationTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var createOrganizationResult = await _mediator.Send(createOrganizationRequest);

        // Act
        var getOrganizationRequest = new GetOrganizationQuery(createOrganizationResult.Value.Id);
        var result = await _mediator.Send(getOrganizationRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(createOrganizationResult.Value.Id);
        result.Value.Name.Should().Be(createOrganizationResult.Value.Name);
        result.Value.Description.Should().Be(createOrganizationResult.Value.Description);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var getOrganizationRequest = new GetOrganizationQuery(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(getOrganizationRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(OrganizationErrors.OrganizationNotFound);
    }
}