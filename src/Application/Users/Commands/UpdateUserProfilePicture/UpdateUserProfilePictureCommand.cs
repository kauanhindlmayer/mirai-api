using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands.UpdateUserProfilePicture;

public sealed record UpdateUserProfilePictureCommand(IFormFile File)
    : IRequest<ErrorOr<Success>>;