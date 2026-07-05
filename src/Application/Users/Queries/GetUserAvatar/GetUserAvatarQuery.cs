using Application.Abstractions.Storage;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetUserAvatar;

public sealed record GetUserAvatarQuery(Guid UserId) : IRequest<ErrorOr<FileResponse>>;
