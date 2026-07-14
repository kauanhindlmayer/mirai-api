using Application.Abstractions;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserProfile;

internal sealed class GetUserProfileQueryHandler
    : IRequestHandler<GetUserProfileQuery, ErrorOr<UserProfileResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetUserProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<UserProfileResponse>> Handle(
        GetUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == query.UserId
                && u.OrganizationMemberships.Any(m => m.OrganizationId == query.OrganizationId))
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.ImageFileId,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFound;
        }

        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId
                && p.Members.Any(m => m.UserId == query.UserId))
            .Select(p => new ProfileProjectResponse(
                p.Id,
                p.Name,
                p.Members.First(m => m.UserId == query.UserId).Role.Name))
            .ToListAsync(cancellationToken);
        var teams = await _context.Teams
            .AsNoTracking()
            .Where(t => t.Project.OrganizationId == query.OrganizationId
                && t.Members.Any(m => m.UserId == query.UserId))
            .Select(t => new ProfileTeamResponse(
                t.Id,
                t.Name,
                t.Project.Name,
                t.Members.First(m => m.UserId == query.UserId).Role.Name))
            .ToListAsync(cancellationToken);
        return new UserProfileResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.ImageFileId != null ? $"/api/users/{user.Id}/avatar" : null,
            projects,
            teams);
    }
}
