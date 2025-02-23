using Application.Common.Interfaces.Persistence;
using Application.WorkItems.Commands.AddTag;
using Domain.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class AddTagTests
{
    private static readonly AddTagCommand Command = new(
        Guid.NewGuid(),
        "Tag Name");

    private readonly AddTagCommandHandler _handler;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly ITagsRepository _tagsRepository;

    public AddTagTests()
    {
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _tagsRepository = Substitute.For<ITagsRepository>();
        _handler = new AddTagCommandHandler(
            _workItemsRepository,
            _tagsRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemsRepository.GetByIdWithTagsAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldCreateTag()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        _workItemsRepository.GetByIdWithTagsAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _tagsRepository.GetByNameAsync(Command.TagName, Arg.Any<CancellationToken>())
            .Returns(null as Tag);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        await _tagsRepository.Received(1).GetByNameAsync(Command.TagName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldAddTagToWorkItem()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        var tag = new Tag(Command.TagName, string.Empty, string.Empty);
        _workItemsRepository.GetByIdWithTagsAsync(Command.WorkItemId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _tagsRepository.GetByNameAsync(Command.TagName, Arg.Any<CancellationToken>())
            .Returns(tag);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        await _tagsRepository.Received(1).GetByNameAsync(Command.TagName, Arg.Any<CancellationToken>());
        workItem.Tags.Should().Contain(tag);
    }
}