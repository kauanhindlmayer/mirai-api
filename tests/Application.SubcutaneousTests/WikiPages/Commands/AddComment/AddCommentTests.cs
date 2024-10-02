using Domain.WikiPages;
using TestCommon.Organizations;
using TestCommon.Projects;
using TestCommon.WikiPages;

namespace Application.SubcutaneousTests.WikiPages.Commands.AddComment;

[Collection(WebAppFactoryCollection.CollectionName)]
public class AddCommentTests(WebAppFactory webAppFactory)
{
    private readonly IMediator _mediator = webAppFactory.CreateMediator();

    [Fact(Skip = "Not implemented")]
    public async Task AddComment_WhenValidCommand_ShouldAddComment()
    {
        // Arrange
        var createOrganizationRequest = OrganizationCommandFactory.CreateCreateOrganizationCommand();
        var organization = await _mediator.Send(createOrganizationRequest);
        var createProjectRequest = ProjectCommandFactory.CreateCreateProjectCommand(organizationId: organization.Value.Id);
        var project = await _mediator.Send(createProjectRequest);
        var createWikiPageRequest = WikiPageCommandFactory.CreateCreateWikiPageCommand(projectId: project.Value.Id);
        var wikiPage = await _mediator.Send(createWikiPageRequest);
        var addCommentRequest = WikiPageCommandFactory.CreateAddCommentCommand(wikiPageId: wikiPage.Value.Id);

        // Act
        var result = await _mediator.Send(addCommentRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.WikiPageId.Should().Be(addCommentRequest.WikiPageId);
        result.Value.Content.Should().Be(addCommentRequest.Content);
    }

    [Fact]
    public async Task AddComment_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var addCommentRequest = WikiPageCommandFactory.CreateAddCommentCommand(wikiPageId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(addCommentRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(WikiPageErrors.WikiPageNotFound);
    }
}