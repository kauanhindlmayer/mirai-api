using Mirai.Application.Organizations.Queries.ListOrganizations;
using Mirai.Application.Projects.Queries.ListProjects;
using TestCommon.Organizations;
using TestCommon.Projects;

namespace Mirai.Application.SubcutaneousTests.Projects.Queries.ListProjects;

[Collection(WebAppFactoryCollection.CollectionName)]
public class ListProjectsTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task ListProjects_WhenProjectsExist_ShouldReturnProjects()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var createProjectResult = await _mediator.Send(createProjectRequest);

        // Act
        var listProjectsRequest = new ListProjectsQuery();
        var result = await _mediator.Send(listProjectsRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(createProjectResult.Value);
    }

    [Fact]
    public async Task ListProjects_WhenNoProjectsExist_ShouldReturnEmptyList()
    {
        // Act
        var listProjectsRequest = new ListProjectsQuery();
        var result = await _mediator.Send(listProjectsRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}