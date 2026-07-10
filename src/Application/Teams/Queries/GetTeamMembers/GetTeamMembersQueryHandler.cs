using Application.Abstractions;
using Application.Abstractions.Mappings;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.GetTeamMembers;

internal sealed class GetTeamMembersQueryHandler
    : IRequestHandler<GetTeamMembersQuery, ErrorOr<PaginatedList<TeamMemberResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetTeamMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<TeamMemberResponse>>> Handle(
        GetTeamMembersQuery query,
        CancellationToken cancellationToken)
    {
        var usersQuery = _context.Users
            .Where(u => u.TeamMemberships.Any(m => m.TeamId == query.TeamId));

        return await usersQuery
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new TeamMemberResponse(
                u.Id,
                u.FullName,
                u.TeamMemberships.First(m => m.TeamId == query.TeamId).RoleId,
                u.TeamMemberships.First(m => m.TeamId == query.TeamId).Role.Name))
            .PaginatedListAsync(
                query.Page,
                query.PageSize,
                cancellationToken);
    }
}
