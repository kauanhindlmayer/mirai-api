using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.GetProjectUsers;

public sealed record GetProjectUsersQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 10,
    string? Sort = null,
    string? SearchTerm = null) : IAuthorizationRequest<ErrorOr<PaginatedList<ProjectUserResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
