using Mirai.Application.Organizations.Queries.GetOrganization;
using Mirai.Application.Organizations.Queries.ListOrganizations;
using Mirai.Domain.Organizations;
using TestCommon.Organizations;

namespace Mirai.Application.SubcutaneousTests.Organizations.Queries.ListOrganizations;

[Collection(WebAppFactoryCollection.CollectionName)]
public class ListOrganizationsTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task ListOrganizations_WhenOrganizationsExist_ShouldReturnOrganizations()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var createOrganizationResult = await _mediator.Send(createOrganizationRequest);

        // Act
        var listOrganizationsRequest = new ListOrganizationsQuery();
        var result = await _mediator.Send(listOrganizationsRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(createOrganizationResult.Value);
    }

    [Fact]
    public async Task ListOrganizations_WhenNoOrganizationsExist_ShouldReturnEmptyList()
    {
        // Act
        var listOrganizationsRequest = new ListOrganizationsQuery();
        var result = await _mediator.Send(listOrganizationsRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}