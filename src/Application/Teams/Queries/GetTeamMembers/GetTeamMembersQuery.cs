using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Teams.Queries.GetTeamMembers;

public sealed record GetTeamMembersQuery(
    Guid TeamId,
    int Page = 1,
    int PageSize = 10) : IAuthorizationRequest<ErrorOr<PaginatedList<TeamMemberResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
