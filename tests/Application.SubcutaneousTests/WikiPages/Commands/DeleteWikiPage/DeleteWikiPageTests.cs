using Domain.WikiPages;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Commands.DeleteWikiPage;

[Collection(WebAppFactoryCollection.CollectionName)]
public class DeleteWikiPageTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task DeleteWikiPage_WhenValidCommand_ShouldDeleteWikiPage()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var deleteWikiPageRequest = WikiPageCommandFactory.CreateDeleteWikiPageCommand(wikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(deleteWikiPageRequest);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteWikiPage_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var deleteWikiPageRequest = WikiPageCommandFactory.CreateDeleteWikiPageCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(deleteWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.WikiPageNotFound);
    }

    [Fact]
    public async Task DeleteWikiPage_WhenWikiPageHasChildren_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var createChildWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: project.Value.Id,
            title: "Child Wiki Page",
            parentWikiPageId: wikiPage.Value.Id);
        await _mediator.Send(createChildWikiPageRequest);
        var deleteWikiPageRequest = WikiPageCommandFactory.CreateDeleteWikiPageCommand(wikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(deleteWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.WikiPageHasSubWikiPages);
    }
}