using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
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
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IUserContext _userContext;

    public AddCommentTests()
    {
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new AddCommentCommandHandler(
            _workItemsRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemsRepository.GetByIdWithCommentsAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemExists_ShouldAddComment()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemsRepository.GetByIdWithCommentsAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _userContext.UserId.Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        workItem.Comments.Should().ContainSingle();
        _workItemsRepository.Received(1).Update(workItem);
    }
}