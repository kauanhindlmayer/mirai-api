using Application.WorkItems.Commands.UnassignWorkItem;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class UnassignWorkItemTests
{
    private static readonly UnassignWorkItemCommand Command = new(Guid.NewGuid());

    private readonly UnassignWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;

    public UnassignWorkItemTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new UnassignWorkItemCommandHandler(_workItemRepository);
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
    public async Task Handle_WhenWorkItemIsAssigned_ShouldClearAssignee()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        workItem.UpdateAssignment(Guid.NewGuid());
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        workItem.AssigneeId.Should().BeNull();
        _workItemRepository.Received().Update(workItem);
    }
}
