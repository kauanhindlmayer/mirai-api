using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.WikiPages.Commands.AddComment;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class AddCommentTests
{
    private static readonly AddCommentCommand Command = new(
        Guid.NewGuid(),
        "Content");

    private readonly AddCommentCommandHandler _handler;
    private readonly IWikiPagesRepository _wikiPagesRepository;
    private readonly IUserContext _userContext;

    public AddCommentTests()
    {
        _wikiPagesRepository = Substitute.For<IWikiPagesRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new AddCommentCommandHandler(
            _wikiPagesRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _wikiPagesRepository.GetByIdAsync(Command.WikiPageId, Arg.Any<CancellationToken>())
            .Returns(null as WikiPage);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

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
        _wikiPagesRepository.GetByIdAsync(Command.WikiPageId, Arg.Any<CancellationToken>())
            .Returns(wikiPage);
        _userContext.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        wikiPage.Comments.Should().ContainSingle();
        wikiPage.Comments.First().Content.Should().Be(Command.Content);
    }
}