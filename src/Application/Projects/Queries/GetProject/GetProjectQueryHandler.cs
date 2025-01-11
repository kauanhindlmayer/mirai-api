using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProject;

internal sealed class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ErrorOr<ProjectResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<ProjectResponse>> Handle(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == query.ProjectId)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OrganizationId = p.OrganizationId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        return project;
    }
}