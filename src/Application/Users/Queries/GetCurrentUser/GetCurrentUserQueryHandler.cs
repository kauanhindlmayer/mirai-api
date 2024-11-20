using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IUserContext userContext,
    IUsersRepository usersRepository)
    : IRequestHandler<GetCurrentUserQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(
            userContext.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        return user;
    }
}
