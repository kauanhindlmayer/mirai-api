using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.ResolveProjectUsers;

public sealed record ResolveProjectUsersQuery(
    Guid ProjectId,
    IReadOnlyList<Guid> UserIds) : IAuthorizationRequest<ErrorOr<IReadOnlyList<ResolvedUserResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
