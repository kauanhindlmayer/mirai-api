using Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authorization.Queries.ListRoles;

internal sealed class ListRolesQueryHandler
    : IRequestHandler<ListRolesQuery, ErrorOr<IReadOnlyList<RoleResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<RoleResponse>>> Handle(
        ListRolesQuery query,
        CancellationToken cancellationToken)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Where(r => query.Scope == null || r.Scope == query.Scope)
            .OrderBy(r => r.Scope)
            .ThenBy(r => r.Name)
            .Select(r => new RoleResponse(r.Id, r.Name, r.Scope))
            .ToListAsync(cancellationToken);

        return roles;
    }
}
