using Application.Abstractions;
using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ErrorOr<UserResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IApplicationDbContext _context;

    public GetCurrentUserQueryHandler(
        IUserContext userContext,
        IApplicationDbContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == _userContext.UserId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FullName,
                ImageUrl = u.ImageUrl,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        return user;
    }
}
