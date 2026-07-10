using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using Application.Projects.Queries.GetProject;
using Domain.Authorization;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.ListProjects;

internal sealed class ListProjectsQueryHandler
    : IRequestHandler<ListProjectsQuery, ErrorOr<IReadOnlyList<ProjectResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IPermissionService _permissionService;

    public ListProjectsQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IPermissionService permissionService)
    {
        _context = context;
        _userContext = userContext;
        _permissionService = permissionService;
    }

    public async Task<ErrorOr<IReadOnlyList<ProjectResponse>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var canViewAllProjects = await _permissionService.HasPermissionAsync(
            _userContext.UserId,
            Permission.ProjectView,
            ResourceType.Organization,
            query.OrganizationId,
            cancellationToken);

        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId
                        && (canViewAllProjects || p.Members.Any(m => m.UserId == _userContext.UserId)))
            .Select(ProjectQueries.ProjectToDto())
            .ToListAsync(cancellationToken);

        return projects;
    }
}
