using Application.Users.Common;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

public sealed record LoginUserQuery(string Email, string Password)
    : IRequest<ErrorOr<AccessToken>>;