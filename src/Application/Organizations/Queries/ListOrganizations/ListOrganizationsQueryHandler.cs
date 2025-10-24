using Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.ListOrganizations;

internal sealed class ListOrganizationsQueryHandler
    : IRequestHandler<ListOrganizationsQuery, ErrorOr<IReadOnlyList<OrganizationBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<OrganizationBriefResponse>>> Handle(
        ListOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Users.Any(u => u.Id == query.UserId))
            .OrderBy(o => o.Name)
            .Select(OrganizationQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return organizations;
    }
}
