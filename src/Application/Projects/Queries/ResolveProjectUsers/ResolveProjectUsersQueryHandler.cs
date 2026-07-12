using Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.ResolveProjectUsers;

internal sealed class ResolveProjectUsersQueryHandler
    : IRequestHandler<ResolveProjectUsersQuery, ErrorOr<IReadOnlyList<ResolvedUserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ResolveProjectUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<ResolvedUserResponse>>> Handle(
        ResolveProjectUsersQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId = await _context.Projects
            .Where(p => p.Id == query.ProjectId)
            .Select(p => p.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => query.UserIds.Contains(u.Id))
            .Where(u => u.OrganizationMemberships.Any(m => m.OrganizationId == organizationId))
            .Select(u => new ResolvedUserResponse(
                u.Id,
                u.FullName,
                u.ImageFileId != null ? $"/api/users/{u.Id}/avatar" : null))
            .ToListAsync(cancellationToken);

        return users;
    }
}
