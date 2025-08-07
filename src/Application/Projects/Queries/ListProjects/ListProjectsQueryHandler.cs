using Application.Abstractions;
using Application.Abstractions.Authentication;
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
    private readonly IUserContext _userContext;

    public ListProjectsQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<ErrorOr<IReadOnlyList<ProjectResponse>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId
                        && p.Users.Any(u => u.Id == _userContext.UserId))
            .Select(ProjectQueries.ProjectToDto())
            .ToListAsync(cancellationToken);

        return projects;
    }
}
