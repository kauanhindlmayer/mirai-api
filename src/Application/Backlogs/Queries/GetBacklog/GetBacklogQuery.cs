using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Backlogs;
using ErrorOr;

namespace Application.Backlogs.Queries.GetBacklog;

/// <summary>
/// Retrieves a team's work items for one sprint, or - when no sprint is given -
/// the team's backlog: the work items that belong to no sprint at all.
/// </summary>
public sealed record GetBacklogQuery(
    Guid TeamId,
    Guid? SprintId,
    BacklogLevel? BacklogLevel) : IAuthorizationRequest<ErrorOr<IReadOnlyList<BacklogResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
