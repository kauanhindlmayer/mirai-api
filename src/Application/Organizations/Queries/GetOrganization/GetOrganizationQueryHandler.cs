using Application.Common.Interfaces.Persistence;
using Application.Organizations.Queries.Common;
using Domain.Organizations;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.GetOrganization;

internal sealed class GetOrganizationQueryHandler
    : IRequestHandler<GetOrganizationQuery, ErrorOr<OrganizationResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<OrganizationResponse>> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Id == query.OrganizationId)
            .Select(OrganizationQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        return organization;
    }
}