using Application.Abstractions.Authentication;
using Application.WorkItems.Commands.UpdateComment;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class UpdateCommentTests
{
    private static readonly UpdateCommentCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Updated content");

    private readonly UpdateCommentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserContext _userContext;

    public UpdateCommentTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new UpdateCommentCommandHandler(_workItemRepository, _userContext);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithCommentsAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCommentDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithCommentsAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WorkItemErrors.CommentNotFound);
    }

    [Fact]
    public async Task Handle_WhenCommentExists_ShouldUpdateComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var comment = new WorkItemComment(workItem.Id, userId, "Original content");
        workItem.AddComment(comment);
        _userContext.UserId.Returns(userId);

        _workItemRepository.GetByIdWithCommentsAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        var updateCommand = new UpdateCommentCommand(
            Command.WorkItemId,
            comment.Id,
            "Updated content");

        // Act
        var result = await _handler.Handle(
            updateCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        comment.Content.Should().Be("Updated content");
        _workItemRepository.Received(1).Update(workItem);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnComment_ShouldReturnError()
    {
        // Arrange
        var commentAuthorId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var comment = new WorkItemComment(workItem.Id, commentAuthorId, "Original content");
        workItem.AddComment(comment);
        _userContext.UserId.Returns(currentUserId);

        _workItemRepository.GetByIdWithCommentsAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        var updateCommand = new UpdateCommentCommand(
            Command.WorkItemId,
            comment.Id,
            "Updated content");

        // Act
        var result = await _handler.Handle(
            updateCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WorkItemErrors.CommentNotOwned);
        comment.Content.Should().NotBe("Updated content");
    }
}