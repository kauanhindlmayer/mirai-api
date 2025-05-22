using Application.Common.Interfaces.Persistence;
using Application.Sprints.Commands.AddWorkItemToSprint;
using Domain.Sprints;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.Sprints.Commands;

public class AddWorkItemToSprintTests
{
    private static readonly AddWorkItemToSprintCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly AddWorkItemToSprintCommandHandler _handler;
    private readonly ISprintsRepository _sprintsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;

    public AddWorkItemToSprintTests()
    {
        _sprintsRepository = Substitute.For<ISprintsRepository>();
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _handler = new AddWorkItemToSprintCommandHandler(
            _sprintsRepository,
            _workItemsRepository);
    }

    [Fact]
    public async Task Handle_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _sprintsRepository.GetByIdAsync(Command.SprintId, TestContext.Current.CancellationToken)
            .Returns(null as Sprint);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var sprint = new Sprint(
            Guid.NewGuid(),
            "Name",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(14)));
        _sprintsRepository.GetByIdAsync(Command.SprintId, TestContext.Current.CancellationToken)
            .Returns(sprint);

        _workItemsRepository.GetByIdAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWorkItemAlreadyInSprint_ShouldReturnError()
    {
        // Arrange
        var sprint = new Sprint(
            Guid.NewGuid(),
            "Name",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(14)));
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Name", WorkItemType.UserStory);
        sprint.AddWorkItem(workItem);

        _sprintsRepository.GetByIdAsync(Command.SprintId, TestContext.Current.CancellationToken)
            .Returns(sprint);

        _workItemsRepository.GetByIdAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SprintErrors.WorkItemAlreadyInSprint);
    }

    [Fact]
    public async Task Handle_WhenWorkItemNotInSprint_ShouldAddWorkItemToSprint()
    {
        // Arrange
        var sprint = new Sprint(
            Guid.NewGuid(),
            "Name",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(14)));
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Name", WorkItemType.UserStory);

        _sprintsRepository.GetByIdAsync(Command.SprintId, TestContext.Current.CancellationToken)
            .Returns(sprint);

        _workItemsRepository.GetByIdAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        sprint.WorkItems.Should().Contain(workItem);
        _sprintsRepository.Received().Update(sprint);
    }
}