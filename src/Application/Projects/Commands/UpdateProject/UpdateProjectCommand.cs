using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid OrganizationId,
    Guid ProjectId,
    string Name,
    string Description) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManage;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
