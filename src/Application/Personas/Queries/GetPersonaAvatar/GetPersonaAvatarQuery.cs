using Application.Abstractions.Authorization;
using Application.Abstractions.Storage;
using Domain.Authorization;
using ErrorOr;

namespace Application.Personas.Queries.GetPersonaAvatar;

public sealed record GetPersonaAvatarQuery(Guid PersonaId) : IAuthorizationRequest<ErrorOr<FileResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Persona;
    public Guid ResourceId => PersonaId;
}
