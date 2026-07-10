using Application.Abstractions.Authorization;
using Application.Abstractions.GitHub;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.GetGitHubInstallationRepositories;

public sealed record GetGitHubInstallationRepositoriesQuery(
    Guid OrganizationId,
    Guid ProjectId,
    long InstallationId,
    string State) : IAuthorizationRequest<ErrorOr<IReadOnlyList<GitHubRepositorySummary>>>
{
    public Permission RequiredPermission => Permission.ProjectManage;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
