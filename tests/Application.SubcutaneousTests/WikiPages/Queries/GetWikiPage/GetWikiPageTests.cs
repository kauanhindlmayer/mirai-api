using Application.WikiPages.Queries.GetWikiPage;
using Domain.WikiPages;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Queries.GetWikiPage;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetWikiPageTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task GetWikiPage_WhenValidQuery_ShouldReturnWikiPage()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var getWikiPageQuery = new GetWikiPageQuery(wikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(getWikiPageQuery);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(wikiPage.Value.Id);
        result.Value.ProjectId.Should().Be(wikiPage.Value.ProjectId);
        result.Value.Title.Should().Be(wikiPage.Value.Title);
        result.Value.Content.Should().Be(wikiPage.Value.Content);
    }

    [Fact]
    public async Task GetWikiPage_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var getWikiPageQuery = new GetWikiPageQuery(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(getWikiPageQuery);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.WikiPageNotFound);
    }
}