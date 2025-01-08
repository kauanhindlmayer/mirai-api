using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfilePicture;

public sealed record UpdateUserProfilePictureCommand(
    Stream Stream,
    string ContentType)
    : IRequest<ErrorOr<Success>>;