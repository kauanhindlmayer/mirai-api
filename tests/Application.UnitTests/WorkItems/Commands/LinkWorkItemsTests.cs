using Application.WorkItems.Commands.LinkWorkItems;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class LinkWorkItemsTests
{
    private static readonly Guid SourceWorkItemId = Guid.NewGuid();
    private static readonly Guid TargetWorkItemId = Guid.NewGuid();

    private static readonly LinkWorkItemsCommand Command = new(
        SourceWorkItemId,
        TargetWorkItemId,
        WorkItemLinkType.Related,
        "Link comment");

    private readonly LinkWorkItemsCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;

    public LinkWorkItemsTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new LinkWorkItemsCommandHandler(_workItemRepository);
    }

    [Fact]
    public async Task Handle_WhenSourceWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdAsync(
            Command.SourceWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTargetWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var sourceWorkItem = new WorkItem(Guid.NewGuid(), 1, "Source", WorkItemType.UserStory);
        _workItemRepository.GetByIdAsync(
            Command.SourceWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(sourceWorkItem);
        _workItemRepository.GetByIdAsync(
            Command.TargetWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.TargetWorkItemNotFound);
    }

    [Fact]
    public async Task Handle_WhenBothWorkItemsExist_ShouldCreateLink()
    {
        // Arrange
        var sourceWorkItem = new WorkItem(Guid.NewGuid(), 1, "Source", WorkItemType.UserStory);
        var targetWorkItem = new WorkItem(Guid.NewGuid(), 2, "Target", WorkItemType.Bug);

        _workItemRepository.GetByIdAsync(
            Command.SourceWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(sourceWorkItem);
        _workItemRepository.GetByIdAsync(
            Command.TargetWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(targetWorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        sourceWorkItem.OutgoingLinks.Should().ContainSingle();
        _workItemRepository.Received(1).Update(sourceWorkItem);
    }

    [Fact]
    public async Task Handle_WhenLinkingToSelf_ShouldReturnError()
    {
        // Arrange
        var workItemId = Guid.NewGuid();
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Work Item", WorkItemType.UserStory);
        var selfLinkCommand = new LinkWorkItemsCommand(
            workItemId,
            workItemId,
            WorkItemLinkType.Related,
            null);

        _workItemRepository.GetByIdAsync(
            workItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            selfLinkCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.CannotLinkToSelf);
        workItem.OutgoingLinks.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDuplicateLink_ShouldReturnError()
    {
        // Arrange
        var sourceWorkItem = new WorkItem(Guid.NewGuid(), 1, "Source", WorkItemType.UserStory);
        var targetWorkItem = new WorkItem(Guid.NewGuid(), 2, "Target", WorkItemType.Bug);

        var existingLink = new WorkItemLink(
            Command.SourceWorkItemId,
            Command.TargetWorkItemId,
            Command.LinkType,
            null);
        sourceWorkItem.AddLink(existingLink);

        _workItemRepository.GetByIdAsync(
            Command.SourceWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(sourceWorkItem);
        _workItemRepository.GetByIdAsync(
            Command.TargetWorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(targetWorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.LinkAlreadyExists);
        sourceWorkItem.OutgoingLinks.Should().ContainSingle();
    }
}
