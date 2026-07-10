using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Commands.DisconnectGitHubRepository;

public sealed record DisconnectGitHubRepositoryCommand(
    Guid OrganizationId,
    Guid ProjectId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManage;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
