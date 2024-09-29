using Application.Authentication.Common;
using Application.Common.Interfaces;
using Domain.Common;
using ErrorOr;
using MediatR;

namespace Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user is null || !user.IsCorrectPasswordHash(query.Password, passwordHasher))
        {
            return AuthenticationErrors.InvalidCredentials;
        }

        var token = jwtTokenGenerator.GenerateToken(
            id: user.Id,
            firstName: user.FirstName,
            lastName: user.LastName,
            email: user.Email,
            permissions: [],
            roles: []);

        return new AuthenticationResult(user, token);
    }
}