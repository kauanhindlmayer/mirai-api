using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, ErrorOr<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return UserErrors.AlreadyExists;
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
        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}