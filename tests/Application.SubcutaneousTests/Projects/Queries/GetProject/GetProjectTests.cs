using Application.Projects.Queries.GetProject;
using Domain.Projects;
using TestCommon.Organizations;
using TestCommon.Projects;

namespace Application.SubcutaneousTests.Projects.Queries.GetProject;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetProjectTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task GetProject_WhenProjectExists_ShouldReturnProject()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);

        // Act
        var getProjectRequest = new GetProjectQuery(project.Value.Id);
        var result = await _mediator.Send(getProjectRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(project.Value.Id);
        result.Value.Name.Should().Be(project.Value.Name);
        result.Value.Description.Should().Be(project.Value.Description);
    }

    [Fact]
    public async Task GetProject_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var getProjectRequest = new GetProjectQuery(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(getProjectRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }
}