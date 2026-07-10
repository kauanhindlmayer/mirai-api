using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.GetGitHubInstallUrl;

public sealed record GetGitHubInstallUrlQuery(
    Guid OrganizationId,
    Guid ProjectId) : IAuthorizationRequest<ErrorOr<string>>
{
    public Permission RequiredPermission => Permission.ProjectManage;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
