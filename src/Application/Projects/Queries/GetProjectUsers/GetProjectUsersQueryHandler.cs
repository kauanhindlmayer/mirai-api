using Application.Abstractions;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectUsers;

internal sealed class GetProjectUsersQueryHandler
    : IRequestHandler<GetProjectUsersQuery, ErrorOr<PaginatedList<ProjectUserResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;

    public GetProjectUsersQueryHandler(
        IApplicationDbContext context,
        SortMappingProvider sortMappingProvider)
    {
        _context = context;
        _sortMappingProvider = sortMappingProvider;
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

        var sortMappings = _sortMappingProvider.GetMappings<ProjectUserResponse, User>();

        return await query
            .ApplySorting(request.Sort, sortMappings, nameof(User.FirstName))
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