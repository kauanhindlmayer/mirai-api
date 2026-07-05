using ErrorOr;
using MediatR;

namespace Application.Users.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<ErrorOr<Success>>;
