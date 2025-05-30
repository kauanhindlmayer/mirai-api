using Application.Users.Queries.Common;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.LoginUser;

public sealed record LoginUserQuery(string Email, string Password)
    : IRequest<ErrorOr<AccessTokenResponse>>;