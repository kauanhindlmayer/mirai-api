using Application.Common.Interfaces.Persistence;
using Application.Projects.Queries.Common;
using Application.Projects.Queries.GetProject;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.ListProjects;

internal sealed class ListProjectsQueryHandler
    : IRequestHandler<ListProjectsQuery, ErrorOr<IReadOnlyList<ProjectResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListProjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<ProjectResponse>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId)
            .Select(ProjectQueries.ProjectToDto())
            .ToListAsync(cancellationToken);

        return projects;
    }
}
