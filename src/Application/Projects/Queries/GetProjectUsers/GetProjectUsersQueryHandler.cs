using Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectUsers;

internal sealed class GetProjectUsersQueryHandler
    : IRequestHandler<GetProjectUsersQuery, ErrorOr<List<ProjectUserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<ProjectUserResponse>>> Handle(
        GetProjectUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Where(u => u.Projects.Any(p => p.Id == request.ProjectId));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(u =>
                EF.Functions.Like(u.FirstName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.LastName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like((u.FirstName + " " + u.LastName).ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.Email.ToLower(), $"%{searchTerm}%"));
        }

        var users = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new ProjectUserResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.ImageUrl))
            .ToListAsync(cancellationToken);

        return users;
    }
}