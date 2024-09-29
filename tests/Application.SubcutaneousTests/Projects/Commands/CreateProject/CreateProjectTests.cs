using Domain.Organizations;
using Domain.Projects;
using TestCommon.Organizations;
using TestCommon.Projects;

namespace Application.SubcutaneousTests.Projects.Commands.CreateProject;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CreateProjectTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task CreateProject_WhenValidCommand_ShouldCreateProject()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);

        // Act
        var result = await _mediator.Send(createProjectRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(createProjectRequest.Name);
        result.Value.Description.Should().Be(createProjectRequest.Description);
    }

    [Fact]
    public async Task CreateProject_WhenProjectWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        await _mediator.Send(createProjectRequest);

        // Act
        var result = await _mediator.Send(createProjectRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.ProjectWithSameNameAlreadyExists);
    }

    [Fact]
    public async Task CreateProject_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(createProjectRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(OrganizationErrors.OrganizationNotFound);
    }
}