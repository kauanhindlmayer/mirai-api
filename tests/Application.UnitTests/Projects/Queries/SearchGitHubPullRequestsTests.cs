using Application.Abstractions.GitHub;
using Application.Projects.Queries.SearchGitHubPullRequests;
using Domain.Projects;

namespace Application.UnitTests.Projects.Queries;

public class SearchGitHubPullRequestsTests
{
    private static readonly Guid ProjectId = Guid.NewGuid();

    private static readonly SearchGitHubPullRequestsQuery Query = new(ProjectId, "fix bug");

    private readonly SearchGitHubPullRequestsQueryHandler _handler;
    private readonly IProjectRepository _projectRepository;
    private readonly IGitHubPullRequestService _pullRequestService;

    public SearchGitHubPullRequestsTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _pullRequestService = Substitute.For<IGitHubPullRequestService>();
        _handler = new SearchGitHubPullRequestsQueryHandler(_projectRepository, _pullRequestService);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProjectHasNoGitHubConnection_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Test Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NoGitHubRepositoryConnected);
    }

    [Fact]
    public async Task Handle_WhenProjectIsConnected_ShouldReturnSearchResults()
    {
        // Arrange
        var project = new Project("Test Project", "Description", Guid.NewGuid());
        project.ConnectGitHubRepository(1001, 2002, "mirai-org", "mirai-app", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        var pullRequests = new List<GitHubPullRequestSummary>
        {
            new(3003, 42, "Fix bug in login", "https://github.com/mirai-org/mirai-app/pull/42", true, false, "octocat"),
        };
        _pullRequestService.SearchPullRequestsAsync(
            1001,
            "mirai-org",
            "mirai-app",
            Query.SearchTerm,
            TestContext.Current.CancellationToken)
            .Returns(pullRequests);

        // Act
        var result = await _handler.Handle(Query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainSingle();
        result.Value.First().Number.Should().Be(42);
    }
}
