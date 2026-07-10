using Application.Abstractions.Authorization;
using Application.Projects.Queries.GetProject;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(Guid OrganizationId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<ProjectResponse>>>
{
    public Permission RequiredPermission => Permission.OrganizationView;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
