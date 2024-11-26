using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName) : IRequest<ErrorOr<Success>>;