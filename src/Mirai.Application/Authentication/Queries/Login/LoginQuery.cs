using ErrorOr;
using MediatR;
using Mirai.Application.Authentication.Common;

namespace Mirai.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;