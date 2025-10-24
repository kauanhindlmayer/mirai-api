using Application.WorkItems.Commands.UnlinkWorkItems;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class UnlinkWorkItemsTests
{
    private static readonly Guid WorkItemId = Guid.NewGuid();
    private static readonly Guid LinkId = Guid.NewGuid();

    private static readonly UnlinkWorkItemsCommand Command = new(
        WorkItemId,
        LinkId);

    private readonly UnlinkWorkItemsCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;

    public UnlinkWorkItemsTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new UnlinkWorkItemsCommandHandler(_workItemRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithLinksAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenLinkDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Work Item", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithLinksAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.LinkNotFound);
    }

    [Fact]
    public async Task Handle_WhenLinkExists_ShouldRemoveLink()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Work Item", WorkItemType.UserStory);
        var link = new WorkItemLink(
            Command.WorkItemId,
            Guid.NewGuid(),
            WorkItemLinkType.Related,
            null);
        workItem.AddLink(link);

        var unlinkCommand = new UnlinkWorkItemsCommand(Command.WorkItemId, link.Id);

        _workItemRepository.GetByIdWithLinksAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            unlinkCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        workItem.OutgoingLinks.Should().BeEmpty();
        _workItemRepository.Received(1).Update(workItem);
    }

    [Fact]
    public async Task Handle_WhenRemovingOneOfMultipleLinks_ShouldOnlyRemoveSpecifiedLink()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Work Item", WorkItemType.UserStory);
        var link1 = new WorkItemLink(
            Command.WorkItemId,
            Guid.NewGuid(),
            WorkItemLinkType.Related,
            null);
        var link2 = new WorkItemLink(
            Command.WorkItemId,
            Guid.NewGuid(),
            WorkItemLinkType.Affects,
            null);

        workItem.AddLink(link1);
        workItem.AddLink(link2);

        var unlinkCommand = new UnlinkWorkItemsCommand(Command.WorkItemId, link1.Id);

        _workItemRepository.GetByIdWithLinksAsync(
            Command.WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(
            unlinkCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Success>>();
        result.IsError.Should().BeFalse();
        workItem.OutgoingLinks.Should().ContainSingle();
        workItem.OutgoingLinks.Should().Contain(link2);
        workItem.OutgoingLinks.Should().NotContain(link1);
        _workItemRepository.Received(1).Update(workItem);
    }
}
