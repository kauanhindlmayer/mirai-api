using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<ErrorOr<User>>;