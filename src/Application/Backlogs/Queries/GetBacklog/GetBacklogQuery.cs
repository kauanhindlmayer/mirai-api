using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Backlogs;
using ErrorOr;

namespace Application.Backlogs.Queries.GetBacklog;

public sealed record GetBacklogQuery(
    Guid TeamId,
    Guid? SprintId,
    BacklogLevel? BacklogLevel) : IAuthorizationRequest<ErrorOr<IReadOnlyList<BacklogResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
