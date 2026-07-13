using Application.Abstractions;
using Application.Abstractions.Mappings;
using Domain.Authorization;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetMentionableProjectUsers;

internal sealed class GetMentionableProjectUsersQueryHandler
    : IRequestHandler<GetMentionableProjectUsersQuery, ErrorOr<PaginatedList<MentionableProjectUserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetMentionableProjectUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<MentionableProjectUserResponse>>> Handle(
        GetMentionableProjectUsersQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId = await _context.Projects
            .Where(p => p.Id == query.ProjectId)
            .Select(p => p.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        var directMemberIds = _context.Projects
            .Where(p => p.Id == query.ProjectId)
            .SelectMany(p => p.Members)
            .Where(m => m.Role.Permissions.Any(rp => rp.Permission == Permission.ProjectView))
            .Select(m => m.UserId);

        var implicitMemberIds = _context.Organizations
            .Where(o => o.Id == organizationId)
            .SelectMany(o => o.Members)
            .Where(m => m.Role.Permissions.Any(rp => rp.Permission == Permission.ProjectView))
            .Select(m => m.UserId);

        var accessibleUserIds = directMemberIds.Union(implicitMemberIds);

        var usersQuery = _context.Users
            .Where(u => accessibleUserIds.Contains(u.Id));

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.FirstName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.LastName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like((u.FirstName + " " + u.LastName).ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(u.Email.ToLower(), $"%{searchTerm}%"));
        }

        return await usersQuery
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new MentionableProjectUserResponse(
                u.Id,
                u.FullName,
                u.ImageFileId != null ? $"/api/users/{u.Id}/avatar" : null))
            .PaginatedListAsync(
                query.Page,
                query.PageSize,
                cancellationToken);
    }
}
