using ErrorOr;
using MediatR;

namespace Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password) : IRequest<ErrorOr<Guid>>;