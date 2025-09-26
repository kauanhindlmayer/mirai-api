using Application.Abstractions.Authentication;
using Application.WikiPages.Commands.UpdateComment;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class UpdateCommentTests
{
    private static readonly UpdateCommentCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Updated content");

    private readonly UpdateCommentCommandHandler _handler;
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IUserContext _userContext;

    public UpdateCommentTests()
    {
        _wikiPageRepository = Substitute.For<IWikiPageRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new UpdateCommentCommandHandler(_wikiPageRepository, _userContext);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPageRepository.GetByIdWithCommentsAsync(
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
    public async Task Handle_WhenCommentDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            "Title",
            "Content",
            Guid.NewGuid());
        _wikiPageRepository.GetByIdWithCommentsAsync(
            Command.WikiPageId,
            TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.CommentNotFound);
    }

    [Fact]
    public async Task Handle_WhenCommentExists_ShouldUpdateComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            "Title",
            "Content",
            Guid.NewGuid());
        var comment = new WikiPageComment(
            wikiPage.Id,
            userId,
            "Original content");
        wikiPage.AddComment(comment);
        _userContext.UserId.Returns(userId);

        _wikiPageRepository.GetByIdWithCommentsAsync(
            Command.WikiPageId,
            TestContext.Current.CancellationToken)
            .Returns(wikiPage);

        var updateCommand = new UpdateCommentCommand(
            Command.WikiPageId,
            comment.Id,
            "Updated content");

        // Act
        var result = await _handler.Handle(
            updateCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        comment.Content.Should().Be("Updated content");
        _wikiPageRepository.Received(1).Update(wikiPage);
    }
}