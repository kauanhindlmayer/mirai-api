using Application.Abstractions.Authentication;
using Application.Users.Queries.Common;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

internal sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, ErrorOr<AccessTokenResponse>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtService _jwtService;

    public LoginUserQueryHandler(
        IUsersRepository usersRepository,
        IJwtService jwtService)
    {
        _usersRepository = usersRepository;
        _jwtService = jwtService;
    }

    public async Task<ErrorOr<AccessTokenResponse>> Handle(
        LoginUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(
            query.Email,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.InvalidCredentials;
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