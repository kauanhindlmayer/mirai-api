using Application.Abstractions.GitHub;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.SearchGitHubPullRequests;

internal sealed class SearchGitHubPullRequestsQueryHandler
    : IRequestHandler<SearchGitHubPullRequestsQuery, ErrorOr<IReadOnlyList<GitHubPullRequestSummary>>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IGitHubPullRequestService _pullRequestService;

    public SearchGitHubPullRequestsQueryHandler(
        IProjectRepository projectRepository,
        IGitHubPullRequestService pullRequestService)
    {
        _projectRepository = projectRepository;
        _pullRequestService = pullRequestService;
    }

    public async Task<ErrorOr<IReadOnlyList<GitHubPullRequestSummary>>> Handle(
        SearchGitHubPullRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var connection = project.GitHubRepositoryConnection;
        if (connection is null)
        {
            return ProjectErrors.NoGitHubRepositoryConnected;
        }

        var results = await _pullRequestService.SearchPullRequestsAsync(
            connection.InstallationId,
            connection.RepositoryOwner,
            connection.RepositoryName,
            query.SearchTerm,
            cancellationToken);

        return results.ToList();
    }
}
