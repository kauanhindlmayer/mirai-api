using Application.Users.Queries.LoginUser;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.LoginWithGitHub;

public sealed record LoginWithGitHubCommand(string Code, string RedirectUri)
    : IRequest<ErrorOr<AccessTokenResponse>>;
