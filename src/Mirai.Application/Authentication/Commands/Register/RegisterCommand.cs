using ErrorOr;
using MediatR;
using Mirai.Application.Authentication.Common;

namespace Mirai.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;