using Application.Abstractions;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Domain.Shared;
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
        GetOrganizationUsersQuery query,
        CancellationToken cancellationToken)
    {
        if (!_sortMappingProvider.ValidateMappings<OrganizationUserResponse, User>(query.Sort))
        {
            return Errors.InvalidSort(query.Sort);
        }

        var usersQuery = _context.Users
            .Where(u => u.Organizations.Any(o => o.Id == query.OrganizationId));

        if (query.ExcludeProjectId.HasValue)
        {
            usersQuery = usersQuery.Where(u =>
                !u.Projects.Any(p => p.Id == query.ExcludeProjectId.Value));
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.FirstName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.LastName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like((u.FirstName + " " + u.LastName).ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.Email.ToLower(), $"%{searchTerm}%"));
        }

        var sortMappings = _sortMappingProvider.GetMappings<OrganizationUserResponse, User>();

        return await usersQuery
            .ApplySorting(query.Sort, sortMappings, nameof(User.FirstName))
            .Select(u => new OrganizationUserResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.ImageUrl,
                u.LastActiveAtUtc))
            .PaginatedListAsync(
                query.Page,
                query.PageSize,
                cancellationToken);
    }
}