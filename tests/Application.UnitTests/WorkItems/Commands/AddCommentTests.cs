using Application.Abstractions.Authentication;
using Application.WorkItems.Commands.AddComment;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class AddCommentTests
{
    private static readonly AddCommentCommand Command = new(
        Guid.NewGuid(),
        "Content");

    private readonly AddCommentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserContext _userContext;

    public AddCommentTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new AddCommentCommandHandler(
            _workItemRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithCommentsAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemExists_ShouldAddComment()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithCommentsAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);
        _userContext.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        workItem.Comments.Should().ContainSingle();
        _workItemRepository.Received(1).Update(workItem);
    }
}