using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands.UpdateUserAvatar;

public sealed record UpdateUserAvatarCommand(IFormFile File) : IRequest<ErrorOr<Success>>;