using Application.Common.Interfaces.Persistence;
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
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IUsersRepository _usersRepository;

    public AssignWorkItemTests()
    {
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _handler = new AssignWorkItemCommandHandler(
            _workItemsRepository,
            _usersRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenAssigneeDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _usersRepository.GetByIdAsync(Command.AssigneeId, Arg.Any<CancellationToken>())
            .Returns(null as User);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.AssigneeNotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemAndAssigneeExist_ShouldAssignWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var assignee = new User("John", "Doe", "john.doe@email.com");
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _usersRepository.GetByIdAsync(Command.AssigneeId, Arg.Any<CancellationToken>())
            .Returns(assignee);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        workItem.AssigneeId.Should().Be(Command.AssigneeId);
    }

    [Fact]
    public async Task Handle_WhenWorkItemIsAssigned_ShouldUpdateWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var assignee = new User("John", "Doe", "john.doe@email.com");
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _usersRepository.GetByIdAsync(Command.AssigneeId, Arg.Any<CancellationToken>())
            .Returns(assignee);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        _workItemsRepository.Received().Update(workItem);
    }
}
