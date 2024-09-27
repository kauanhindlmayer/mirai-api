using ErrorOr;
using MediatR;
using Mirai.Application.Authentication.Common;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Common;
using Mirai.Domain.Users;

namespace Mirai.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
        : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (await usersRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return AuthenticationErrors.UserAlreadyExists;
        }

        var hashPasswordResult = passwordHasher.HashPassword(command.Password);

        if (hashPasswordResult.IsError)
        {
            return hashPasswordResult.Errors;
        }

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email,
            hashPasswordResult.Value);

        await usersRepository.AddAsync(user, cancellationToken);

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