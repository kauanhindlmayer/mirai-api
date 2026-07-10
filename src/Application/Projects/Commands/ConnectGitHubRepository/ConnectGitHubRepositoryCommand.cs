using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Commands.ConnectGitHubRepository;

public sealed record ConnectGitHubRepositoryCommand(
    Guid OrganizationId,
    Guid ProjectId,
    long InstallationId,
    long RepositoryId,
    string RepositoryOwner,
    string RepositoryName) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManage;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
