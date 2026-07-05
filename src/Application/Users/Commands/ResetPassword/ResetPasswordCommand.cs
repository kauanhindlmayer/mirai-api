using ErrorOr;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<ErrorOr<Success>>;
