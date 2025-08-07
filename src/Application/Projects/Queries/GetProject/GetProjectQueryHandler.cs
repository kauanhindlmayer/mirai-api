using Application.Abstractions;
using Application.Abstractions.Authentication;
using Domain.Projects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProject;

internal sealed class GetProjectQueryHandler
    : IRequestHandler<GetProjectQuery, ErrorOr<ProjectResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetProjectQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<ErrorOr<ProjectResponse>> Handle(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == query.ProjectId
                        && p.Users.Any(u => u.Id == _userContext.UserId))
            .Select(ProjectQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        return project;
    }
}