using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.ListOrganizations;

internal sealed class ListOrganizationsQueryHandler
    : IRequestHandler<ListOrganizationsQuery, ErrorOr<List<OrganizationBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<OrganizationBriefResponse>>> Handle(
        ListOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations
            .AsNoTracking()
            .Select(o => new OrganizationBriefResponse
            {
                Id = o.Id,
                Name = o.Name,
            })
            .ToListAsync(cancellationToken);

        return organizations;
    }
}
