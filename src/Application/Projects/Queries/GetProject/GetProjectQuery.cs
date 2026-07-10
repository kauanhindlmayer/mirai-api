using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IAuthorizationRequest<ErrorOr<ProjectResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
