using Application.WorkItems.Commands.RemovePullRequestLink;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class RemovePullRequestLinkTests
{
    private static readonly Guid WorkItemId = Guid.NewGuid();
    private static readonly Guid LinkId = Guid.NewGuid();

    private static readonly RemovePullRequestLinkCommand Command = new(WorkItemId, LinkId);

    private readonly RemovePullRequestLinkCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;

    public RemovePullRequestLinkTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _handler = new RemovePullRequestLinkCommandHandler(_workItemRepository);
    }

    [Fact]
    public async Task Handle_WhenWorkItemDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(null as WorkItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenLinkDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Test Work Item", WorkItemType.UserStory);
        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.PullRequestLinkNotFound);
    }

    [Fact]
    public async Task Handle_WhenLinkExists_ShouldRemoveLink()
    {
        // Arrange
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Test Work Item", WorkItemType.UserStory);
        var link = new WorkItemPullRequestLink(
            workItem.Id,
            3003,
            42,
            "Fixes #1",
            "https://github.com/mirai-org/mirai-app/pull/42",
            PullRequestLinkState.Open,
            "octocat",
            PullRequestLinkSource.Manual);
        workItem.AddPullRequestLink(link);

        var command = new RemovePullRequestLinkCommand(WorkItemId, link.Id);

        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.PullRequestLinks.Should().BeEmpty();
        _workItemRepository.Received(1).Update(workItem);
    }
}
