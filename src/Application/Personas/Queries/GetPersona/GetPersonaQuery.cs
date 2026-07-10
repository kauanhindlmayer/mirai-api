using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Personas.Queries.GetPersona;

public sealed record GetPersonaQuery(Guid PersonaId) : IAuthorizationRequest<ErrorOr<PersonaResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Persona;
    public Guid ResourceId => PersonaId;
}
