using Application.WorkItems.Commands.AssignWorkItem;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class AssignWorkItemTests
{
    private static readonly AssignWorkItemCommand Command = new(
       Guid.NewGuid(),
       Guid.NewGuid());

    private readonly AssignWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserRepository _userRepository;

    public AssignWorkItemTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new AssignWorkItemCommandHandler(
            _workItemRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenAssigneeDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);
        _userRepository.GetByIdAsync(
            Command.AssigneeId,
            TestContext.Current.CancellationToken)
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.AssigneeNotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemAndAssigneeExist_ShouldAssignWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var assignee = new User("John", "Doe", "john.doe@mirai.com");
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);
        _userRepository.GetByIdAsync(
            Command.AssigneeId,
            TestContext.Current.CancellationToken)
            .Returns(assignee);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        workItem.AssignedUserId.Should().Be(Command.AssigneeId);
    }

    [Fact]
    public async Task Handle_WhenWorkItemIsAssigned_ShouldUpdateWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var assignee = new User("John", "Doe", "john.doe@mirai.com");
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);
        _userRepository.GetByIdAsync(
            Command.AssigneeId,
            TestContext.Current.CancellationToken)
            .Returns(assignee);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        _workItemRepository.Received().Update(workItem);
    }
}
