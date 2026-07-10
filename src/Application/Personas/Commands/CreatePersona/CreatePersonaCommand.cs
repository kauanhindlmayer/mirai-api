using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Personas.Commands.CreatePersona;

public sealed record CreatePersonaCommand(
    Guid ProjectId,
    string Name,
    string? Category,
    string? Description,
    IFormFile? File) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManagePersonas;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
