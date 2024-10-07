using Domain.WikiPages;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Commands.UpdateWikiPage;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateWikiPageTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task UpdateWikiPage_WhenValidCommand_ShouldUpdateWikiPage()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var updateWikiPageRequest = WikiPageCommandFactory.CreateUpdateWikiPageCommand(wikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(updateWikiPageRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(wikiPage.Value.Id);
        result.Value.ProjectId.Should().Be(wikiPage.Value.ProjectId);
        result.Value.Title.Should().Be(updateWikiPageRequest.Title);
        result.Value.Content.Should().Be(updateWikiPageRequest.Content);
    }

    [Fact]
    public async Task UpdateWikiPage_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var updateWikiPageRequest = WikiPageCommandFactory.CreateUpdateWikiPageCommand(wikiPageId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(updateWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task UpdateWikiPage_WhenTitleIsTooLong_ShouldReturnError()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var updateWikiPageRequest = WikiPageCommandFactory.CreateUpdateWikiPageCommand(
            wikiPageId: wikiPage.Value.Id,
            title: new string('a', 256));

        // Act
        var result = await _mediator.Send(updateWikiPageRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
    }
}