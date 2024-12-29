using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    string FirstName,
    string LastName) : IRequest<ErrorOr<Success>>;