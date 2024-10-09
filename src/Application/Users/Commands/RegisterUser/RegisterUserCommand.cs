using ErrorOr;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<ErrorOr<Guid>>;