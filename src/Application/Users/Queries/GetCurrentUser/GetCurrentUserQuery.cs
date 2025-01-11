using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<ErrorOr<UserResponse>>;