using Application.Abstractions;
using Application.Abstractions.Mappings;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectUsers;

internal sealed class GetProjectUsersQueryHandler
    : IRequestHandler<GetProjectUsersQuery, ErrorOr<PaginatedList<ProjectUserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<ProjectUserResponse>>> Handle(
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

        return await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new ProjectUserResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.ImageUrl))
            .PaginatedListAsync(
                request.Page,
                request.PageSize,
                cancellationToken);
    }
}