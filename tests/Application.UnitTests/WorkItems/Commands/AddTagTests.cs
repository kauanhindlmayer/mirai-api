using Application.WorkItems.Commands.AddTag;
using Domain.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class AddTagTests
{
    private static readonly AddTagCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Tag Name");

    private readonly AddTagCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ITagRepository _tagsRepository;

    public AddTagTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _tagsRepository = Substitute.For<ITagRepository>();
        _handler = new AddTagCommandHandler(
            _workItemRepository,
            _tagsRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithTagsAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldCreateTag()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithTagsAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);
        _tagsRepository.GetByNameAsync(Command.TagName, TestContext.Current.CancellationToken)
            .Returns(null as Tag);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        await _tagsRepository.Received(1).GetByNameAsync(Command.TagName, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldAddTagToWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var tag = new Tag(Command.TagName, string.Empty, string.Empty);
        _workItemRepository.GetByIdWithTagsAsync(Command.WorkItemId, TestContext.Current.CancellationToken)
            .Returns(workItem);
        _tagsRepository.GetByNameAsync(Command.TagName, TestContext.Current.CancellationToken)
            .Returns(tag);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        await _tagsRepository.Received(1).GetByNameAsync(Command.TagName, TestContext.Current.CancellationToken);
        workItem.Tags.Should().Contain(tag);
    }
}