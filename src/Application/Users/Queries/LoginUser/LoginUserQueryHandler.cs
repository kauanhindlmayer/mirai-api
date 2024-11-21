using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Users.Common;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

public sealed class LoginUserQueryHandler(
    IUsersRepository _usersRepository,
    IJwtService _jwtService)
    : IRequestHandler<LoginUserQuery, ErrorOr<AccessTokenResponse>>
{
    public async Task<ErrorOr<AccessTokenResponse>> Handle(
        LoginUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(
            query.Email,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.AuthenticationFailed;
        }

        var result = await _jwtService.GetAccessTokenAsync(
            query.Email,
            query.Password,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }

        return new AccessTokenResponse(result.Value);
    }
}