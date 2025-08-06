using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Organizations.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.ListOrganizations;

internal sealed class ListOrganizationsQueryHandler
    : IRequestHandler<ListOrganizationsQuery, ErrorOr<IReadOnlyList<OrganizationBriefResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public ListOrganizationsQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<ErrorOr<IReadOnlyList<OrganizationBriefResponse>>> Handle(
        ListOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Users.Any(u => u.Id == _userContext.UserId))
            .OrderBy(o => o.Name)
            .Select(OrganizationQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return organizations;
    }
}
