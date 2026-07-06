using Application.Abstractions;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.GetTeamMembers;

public sealed record GetTeamMembersQuery(
    Guid TeamId,
    int Page = 1,
    int PageSize = 10) : IRequest<ErrorOr<PaginatedList<TeamMemberResponse>>>;
