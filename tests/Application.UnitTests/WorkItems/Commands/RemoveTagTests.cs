using Application.WorkItems.Commands.RemoveTag;
using Domain.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class RemoveTagTests
{
    private static readonly RemoveTagCommand Command = new(
        Guid.NewGuid(),
        "Tag Name");

    private readonly RemoveTagCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;

    public RemoveTagTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new RemoveTagCommandHandler(_workItemRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithTagsAsync(
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
    public async Task Handle_WhenTagDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithTagsAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(TagErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldRemoveTag()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var tag = new Tag("Tag Name", string.Empty, string.Empty);
        workItem.AddTag(tag);
        _workItemRepository.GetByIdWithTagsAsync(
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
        workItem.Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldUpdateWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var tag = new Tag("Tag Name", string.Empty, string.Empty);
        workItem.AddTag(tag);
        _workItemRepository.GetByIdWithTagsAsync(
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
        _workItemRepository.Received(1).Update(workItem);
    }
}