using Application.Abstractions.Authorization;
using Application.Abstractions.GitHub;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.SearchGitHubPullRequests;

public sealed record SearchGitHubPullRequestsQuery(
    Guid ProjectId,
    string SearchTerm) : IAuthorizationRequest<ErrorOr<IReadOnlyList<GitHubPullRequestSummary>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
