using Application.Common.Interfaces.Persistence;
using Application.WorkItems.Commands.DeleteWorkItem;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class DeleteWorkItemTests
{
    private static readonly DeleteWorkItemCommand Command = new(Guid.NewGuid());

    private readonly DeleteWorkItemCommandHandler _handler;
    private readonly IWorkItemsRepository _workItemsRepository;

    public DeleteWorkItemTests()
    {
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _handler = new DeleteWorkItemCommandHandler(_workItemsRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemExists_ShouldDeleteWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemsRepository.GetByIdAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
    }
}