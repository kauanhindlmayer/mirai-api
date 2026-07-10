using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Teams.Queries.GetTeam;

public sealed record GetTeamQuery(Guid TeamId) : IAuthorizationRequest<ErrorOr<TeamResponse>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
