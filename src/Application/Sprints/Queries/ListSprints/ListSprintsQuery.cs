using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Queries.ListSprints;

public sealed record ListSprintsQuery(Guid TeamId) : IAuthorizationRequest<ErrorOr<IReadOnlyList<SprintResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
