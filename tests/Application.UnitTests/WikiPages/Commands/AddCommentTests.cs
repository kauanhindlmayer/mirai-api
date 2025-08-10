using Application.Abstractions.Authentication;
using Application.WikiPages.Commands.AddComment;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class AddCommentTests
{
    private static readonly AddCommentCommand Command = new(
        Guid.NewGuid(),
        "Content");

    private readonly AddCommentCommandHandler _handler;
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IUserContext _userContext;

    public AddCommentTests()
    {
        _wikiPageRepository = Substitute.For<IWikiPageRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new AddCommentCommandHandler(
            _wikiPageRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPageRepository.GetByIdAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(null as WikiPage);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageExists_ShouldAddComment()
    {
        // Arrange
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            "Title",
            "Content",
            Guid.NewGuid());
        _wikiPageRepository.GetByIdAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(wikiPage);
        _userContext.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        wikiPage.Comments.Should().ContainSingle();
        wikiPage.Comments.First().Content.Should().Be(Command.Content);
    }
}