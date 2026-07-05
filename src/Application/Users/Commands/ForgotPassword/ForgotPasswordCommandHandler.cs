using System.Security.Cryptography;
using Application.Abstractions.Email;
using Application.Abstractions.Links;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, ErrorOr<Success>>
{
    private const int TokenLengthInBytes = 64;
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IFrontendLinkService _frontendLinkService;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IFrontendLinkService frontendLinkService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _frontendLinkService = frontendLinkService;
    }

    public async Task<ErrorOr<Success>> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Success;
        }

        var token = RandomNumberGenerator.GetHexString(TokenLengthInBytes);
        user.SetPasswordResetToken(token, DateTime.UtcNow.Add(TokenLifetime));
        _userRepository.Update(user);

        var resetLink = _frontendLinkService.BuildResetPasswordLink(user.Email, token);

        await _emailService.SendEmailAsync(
            user.Email,
            "Reset your password",
            $"Click the link below to reset your password:\n{resetLink}",
            cancellationToken);

        return Result.Success;
    }
}
