using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

internal sealed class LoginUserQueryHandler
    : IRequestHandler<LoginUserQuery, ErrorOr<AccessTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginUserQueryHandler(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<ErrorOr<AccessTokenResponse>> Handle(
        LoginUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
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