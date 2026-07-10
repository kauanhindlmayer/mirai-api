using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Personas.Commands.DeletePersona;

public sealed record DeletePersonaCommand(
    Guid ProjectId,
    Guid PersonaId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManagePersonas;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
