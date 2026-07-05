using Application.Abstractions.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

internal sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<Success>> Handle(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(command.Token, cancellationToken);

        if (user is null
            || user.Email != command.Email
            || user.PasswordResetTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return UserErrors.InvalidOrExpiredPasswordResetToken;
        }

        await _authenticationService.ResetPasswordAsync(
            user.IdentityId,
            command.NewPassword,
            cancellationToken);

        user.ClearPasswordResetToken();
        _userRepository.Update(user);

        return Result.Success;
    }
}
