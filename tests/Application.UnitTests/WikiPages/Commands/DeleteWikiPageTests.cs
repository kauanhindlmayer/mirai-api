using Application.Common.Interfaces.Persistence;
using Application.WikiPages.Commands.DeleteWikiPage;
using Domain.Projects;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class DeleteWikiPageTests
{
    private static readonly DeleteWikiPageCommand Command = new(Guid.NewGuid());

    private readonly DeleteWikiPageCommandHandler _handler;
    private readonly IWikiPagesRepository _wikiPagesRepository;

    public DeleteWikiPageTests()
    {
        _wikiPagesRepository = Substitute.For<IWikiPagesRepository>();
        _handler = new DeleteWikiPageCommandHandler(_wikiPagesRepository);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPagesRepository.GetByIdAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(null as WikiPage);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageHasSubWikiPages_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var parentWikiPage = new WikiPage(project.Id, "Title", "Content", Guid.NewGuid());
        var wikiPage = new WikiPage(
            project.Id,
            "Title",
            "Content",
            Guid.NewGuid(),
            parentWikiPage.Id);
        parentWikiPage.InsertSubWikiPage(wikiPage.SubWikiPages.Count, wikiPage);
        _wikiPagesRepository.GetByIdWithSubWikiPagesAsync(parentWikiPage.Id, TestContext.Current.CancellationToken)
            .Returns(parentWikiPage);

        // Act
        var result = await _handler.Handle(
            Command with { WikiPageId = parentWikiPage.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.HasSubWikiPages);
    }

    [Fact]
    public async Task Handle_WhenWikiPageExists_ShouldDeleteWikiPage()
    {
        // Arrange
        var wikiPage = new WikiPage(Guid.NewGuid(), "Title", "Content", Guid.NewGuid());
        _wikiPagesRepository.GetByIdWithSubWikiPagesAsync(
            Command.WikiPageId,
            TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(Result.Success);
        _wikiPagesRepository.Received().Remove(wikiPage);
    }
}