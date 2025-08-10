using Application.WikiPages.Commands.DeleteComment;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class DeleteCommentTests
{
    private static readonly DeleteCommentCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly DeleteCommentCommandHandler _handler;
    private readonly IWikiPageRepository _wikiPageRepository;

    public DeleteCommentTests()
    {
        _wikiPageRepository = Substitute.For<IWikiPageRepository>();
        _handler = new DeleteCommentCommandHandler(_wikiPageRepository);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPageRepository.GetByIdWithCommentsAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(null as WikiPage);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageExistsAndCommentDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            "Title",
            "Content",
            Guid.NewGuid());
        _wikiPageRepository.GetByIdWithCommentsAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.CommentNotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageExistsAndCommentExists_ShouldRemoveComment()
    {
        // Arrange
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            "Title",
            "Content",
            Guid.NewGuid());
        var comment = new WikiPageComment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Content");
        wikiPage.AddComment(comment);
        _wikiPageRepository.GetByIdWithCommentsAsync(Command.WikiPageId, TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        // Act
        var result = await _handler.Handle(
            Command with { CommentId = comment.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Should().BeOfType<ErrorOr<Success>>();
        wikiPage.Comments.Should().BeEmpty();
        _wikiPageRepository.Received().Update(wikiPage);
    }
}