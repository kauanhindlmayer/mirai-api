using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUsersRepository usersRepository,
    IAuthenticationService authenticationService)
    : IRequestHandler<RegisterUserCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        if (await usersRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return UserErrors.AlreadyExists;
        }

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email);

        var identityId = await authenticationService.RegisterAsync(
            user,
            command.Password,
            cancellationToken);

        user.SetIdentityId(identityId);
        await usersRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}