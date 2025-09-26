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
    private readonly ISprintRepository _sprintRepository;
    private readonly IWorkItemRepository _workItemRepository;

    public AddWorkItemToSprintTests()
    {
        _sprintRepository = Substitute.For<ISprintRepository>();
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new AddWorkItemToSprintCommandHandler(
            _sprintRepository,
            _workItemRepository);
    }

    [Fact]
    public async Task Handle_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _sprintRepository.GetByIdAsync(
            Command.SprintId,
            TestContext.Current.CancellationToken)
            .Returns(null as Sprint);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _sprintRepository.GetByIdAsync(
            Command.SprintId,
            TestContext.Current.CancellationToken)
            .Returns(sprint);
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _sprintRepository.GetByIdAsync(
            Command.SprintId,
            TestContext.Current.CancellationToken)
            .Returns(sprint);
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _sprintRepository.GetByIdAsync(
            Command.SprintId,
            TestContext.Current.CancellationToken)
            .Returns(sprint);
        _workItemRepository.GetByIdAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        sprint.WorkItems.Should().Contain(workItem);
        _sprintRepository.Received().Update(sprint);
    }
}