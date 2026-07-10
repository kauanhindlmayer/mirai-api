using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Personas.Commands.UpdatePersona;

public sealed record UpdatePersonaCommand(
    Guid ProjectId,
    Guid PersonaId,
    string Name,
    string? Description,
    IFormFile? File) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManagePersonas;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
