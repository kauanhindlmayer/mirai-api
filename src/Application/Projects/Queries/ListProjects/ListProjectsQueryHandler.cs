using Application.Common.Interfaces.Persistence;
using Application.Projects.Queries.GetProject;
using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.ListProjects;

internal sealed class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, ErrorOr<List<ProjectResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListProjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<ProjectResponse>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OrganizationId = p.OrganizationId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return projects;
    }
}
