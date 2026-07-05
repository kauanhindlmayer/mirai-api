using Application.Abstractions.Storage;
using ErrorOr;
using MediatR;

namespace Application.Personas.Queries.GetPersonaAvatar;

public sealed record GetPersonaAvatarQuery(Guid PersonaId) : IRequest<ErrorOr<FileResponse>>;
