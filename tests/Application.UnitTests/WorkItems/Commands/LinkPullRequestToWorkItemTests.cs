using Application.Abstractions.GitHub;
using Application.WorkItems.Commands.LinkPullRequestToWorkItem;
using Domain.Projects;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class LinkPullRequestToWorkItemTests
{
    private static readonly Guid WorkItemId = Guid.NewGuid();
    private static readonly Guid ProjectId = Guid.NewGuid();

    private static readonly LinkPullRequestToWorkItemCommand Command = new(WorkItemId, PullRequestNumber: 42);

    private readonly LinkPullRequestToWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IGitHubPullRequestService _pullRequestService;

    public LinkPullRequestToWorkItemTests()
    {
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _projectRepository = Substitute.For<IProjectRepository>();
        _pullRequestService = Substitute.For<IGitHubPullRequestService>();
        _handler = new LinkPullRequestToWorkItemCommandHandler(
            _workItemRepository,
            _projectRepository,
            _pullRequestService);
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
    public async Task Handle_WhenProjectHasNoGitHubConnection_ShouldReturnError()
    {
        // Arrange
        var workItem = CreateWorkItem();
        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        var project = new Project("Test Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NoGitHubRepositoryConnected);
    }

    [Fact]
    public async Task Handle_WhenPullRequestNotFoundOnGitHub_ShouldReturnError()
    {
        // Arrange
        var workItem = CreateWorkItem();
        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        var project = new Project("Test Project", "Description", Guid.NewGuid());
        project.ConnectGitHubRepository(1001, 2002, "mirai-org", "mirai-app", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        _pullRequestService.GetPullRequestAsync(
            1001,
            "mirai-org",
            "mirai-app",
            Command.PullRequestNumber,
            TestContext.Current.CancellationToken)
            .Returns(null as GitHubPullRequestSummary);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.GitHubPullRequestNotFound);
    }

    [Fact]
    public async Task Handle_WhenPullRequestExists_ShouldAddLink()
    {
        // Arrange
        var workItem = CreateWorkItem();
        _workItemRepository.GetByIdWithPullRequestLinksAsync(
            WorkItemId,
            TestContext.Current.CancellationToken)
            .Returns(workItem);

        var project = new Project("Test Project", "Description", Guid.NewGuid());
        project.ConnectGitHubRepository(1001, 2002, "mirai-org", "mirai-app", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        var pullRequest = new GitHubPullRequestSummary(
            Id: 3003,
            Number: Command.PullRequestNumber,
            Title: "Fixes #1",
            HtmlUrl: "https://github.com/mirai-org/mirai-app/pull/42",
            IsOpen: true,
            IsMerged: false,
            AuthorLogin: "octocat");
        _pullRequestService.GetPullRequestAsync(
            1001,
            "mirai-org",
            "mirai-app",
            Command.PullRequestNumber,
            TestContext.Current.CancellationToken)
            .Returns(pullRequest);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        workItem.PullRequestLinks.Should().ContainSingle();
        workItem.PullRequestLinks.First().State.Should().Be(PullRequestLinkState.Open);
        workItem.PullRequestLinks.First().Source.Should().Be(PullRequestLinkSource.Manual);
        _workItemRepository.Received(1).Update(workItem);
    }

    private static WorkItem CreateWorkItem()
    {
        return new WorkItem(ProjectId, 1, "Test Work Item", WorkItemType.UserStory);
    }
}
