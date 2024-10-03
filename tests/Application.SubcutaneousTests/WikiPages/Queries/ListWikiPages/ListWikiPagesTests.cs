using Application.WikiPages.Queries.ListWikiPages;
using Domain.Projects;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Queries.ListWikiPages;

[Collection(WebAppFactoryCollection.CollectionName)]
public class ListWikiPagesTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact]
    public async Task ListWikiPages_WhenValidQuery_ShouldReturnWikiPages()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(
            organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var listWikiPagesQuery = new ListWikiPagesQuery(project.Value.Id);

        // Act
        var result = await _mediator.Send(listWikiPagesQuery);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.First().Id.Should().Be(wikiPage.Value.Id);
        result.Value.First().ProjectId.Should().Be(wikiPage.Value.ProjectId);
        result.Value.First().Title.Should().Be(wikiPage.Value.Title);
        result.Value.First().Content.Should().Be(wikiPage.Value.Content);
    }

    [Fact]
    public async Task ListWikiPages_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var listWikiPagesQuery = new ListWikiPagesQuery(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(listWikiPagesQuery);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task ListWikiPages_WhenNoWikiPages_ShouldReturnEmptyList()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(
            organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var listWikiPagesQuery = new ListWikiPagesQuery(project.Value.Id);

        // Act
        var result = await _mediator.Send(listWikiPagesQuery);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ListWikiPages_WhenMultipleWikiPages_ShouldReturnAllWikiPages()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(
            organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest1 = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: project.Value.Id, title: "WikiPage1");
        var wikiPage1 = await _mediator.Send(createWikiPageRequest1);
        var createWikiPageRequest2 = WikiPageCommandFactory.CreateCreateWikiPageCommand(
            projectId: project.Value.Id, title: "WikiPage2");
        var wikiPage2 = await _mediator.Send(createWikiPageRequest2);
        var listWikiPagesQuery = new ListWikiPagesQuery(project.Value.Id);

        // Act
        var result = await _mediator.Send(listWikiPagesQuery);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.First().Id.Should().Be(wikiPage1.Value.Id);
        result.Value.First().ProjectId.Should().Be(wikiPage1.Value.ProjectId);
        result.Value.First().Title.Should().Be(wikiPage1.Value.Title);
        result.Value.First().Content.Should().Be(wikiPage1.Value.Content);
        result.Value.Last().Id.Should().Be(wikiPage2.Value.Id);
        result.Value.Last().ProjectId.Should().Be(wikiPage2.Value.ProjectId);
        result.Value.Last().Title.Should().Be(wikiPage2.Value.Title);
        result.Value.Last().Content.Should().Be(wikiPage2.Value.Content);
    }
}