using Application.WikiPages.Commands.UpdateWikiPage;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class UpdateWikiPageTests
{
    private static readonly UpdateWikiPageCommand Command = new(
        Guid.NewGuid(),
        "Title",
        "Content");

    private readonly UpdateWikiPageCommandHandler _handler;
    private readonly IWikiPageRepository _wikiPageRepository;

    public UpdateWikiPageTests()
    {
        _wikiPageRepository = Substitute.For<IWikiPageRepository>();
        _handler = new UpdateWikiPageCommandHandler(_wikiPageRepository);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPageRepository.GetByIdAsync(
            Command.WikiPageId,
            TestContext.Current.CancellationToken)
            .Returns(null as WikiPage);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageExists_ShouldUpdateWikiPage()
    {
        // Arrange
        var wikiPage = new WikiPage(Guid.NewGuid(), "Title", "Content", Guid.NewGuid());
        _wikiPageRepository.GetByIdAsync(
            Command.WikiPageId,
            TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(wikiPage.Id);
    }
}