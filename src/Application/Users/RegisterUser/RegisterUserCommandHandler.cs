using Application.Common.Interfaces;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUsersRepository _usersRepository,
    IAuthenticationService _authenticationService)
    : IRequestHandler<RegisterUserCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        if (await _usersRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return UserErrors.UserAlreadyExists;
        }

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email);

        var identityId = await _authenticationService.RegisterAsync(
            user,
            command.Password,
            cancellationToken);

        user.SetIdentityId(identityId);
        await _usersRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}