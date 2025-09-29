using Application.Abstractions;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.GetOrganizationUsers;

internal sealed class GetOrganizationUsersQueryHandler
    : IRequestHandler<GetOrganizationUsersQuery, ErrorOr<PaginatedList<OrganizationUserResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;

    public GetOrganizationUsersQueryHandler(
        IApplicationDbContext context,
        SortMappingProvider sortMappingProvider)
    {
        _context = context;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<ErrorOr<PaginatedList<OrganizationUserResponse>>> Handle(
        GetOrganizationUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Where(u => u.Organizations.Any(o => o.Id == request.OrganizationId));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(u =>
                EF.Functions.Like(u.FirstName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.LastName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like((u.FirstName + " " + u.LastName).ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.Email.ToLower(), $"%{searchTerm}%"));
        }

        var sortMappings = _sortMappingProvider.GetMappings<OrganizationUserResponse, User>();

        return await query
            .ApplySorting(request.Sort, sortMappings, nameof(User.FirstName))
            .Select(u => new OrganizationUserResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.ImageUrl,
                u.LastActiveAtUtc))
            .PaginatedListAsync(
                request.Page,
                request.PageSize,
                cancellationToken);
    }
}