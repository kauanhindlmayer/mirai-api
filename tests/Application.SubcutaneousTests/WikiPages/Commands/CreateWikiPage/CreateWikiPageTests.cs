using Domain.Projects;
using Domain.WikiPages;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Commands.CreateWikiPage;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CreateWikiPageTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task CreateWikiPage_WhenValidCommand_ShouldCreateWikiPage()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);

        // Act
        var result = await _mediator.Send(createWikiPageRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.ProjectId.Should().Be(createWikiPageRequest.ProjectId);
        result.Value.Title.Should().Be(createWikiPageRequest.Title);
        result.Value.Content.Should().Be(createWikiPageRequest.Content);
    }

    [Fact]
    public async Task CreateWikiPage_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(createWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task CreateWikiPage_WhenParentWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id, parentWikiPageId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(createWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
    }

    [Fact]
    public async Task CreateWikiPage_WhenParentWikiPageExists_ShouldCreateWikiPage()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createParentWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: project.Value.Id,
            title: "Parent Wiki Page");
        var parentWikiPage = await _mediator.Send(createParentWikiPageRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id, parentWikiPageId: parentWikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(createWikiPageRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.ProjectId.Should().Be(createWikiPageRequest.ProjectId);
        result.Value.Title.Should().Be(createWikiPageRequest.Title);
        result.Value.Content.Should().Be(createWikiPageRequest.Content);
        result.Value.ParentWikiPageId.Should().Be(createWikiPageRequest.ParentWikiPageId);
    }

    [Fact]
    public async Task CreateWikiPage_WhenParentWikiPageIsNotInSameProject_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createParentWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var parentWikiPage = await _mediator.Send(createParentWikiPageRequest);
        var createOtherProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(
            organizationId: organization.Value.Id,
            name: "Other Project");
        var otherProject = await _mediator.Send(createOtherProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: otherProject.Value.Id,
            parentWikiPageId: parentWikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(createWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
    }
}