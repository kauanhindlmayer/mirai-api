using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Personas.Commands.UpdatePersona;

public sealed record UpdatePersonaCommand(
    Guid ProjectId,
    Guid PersonaId,
    string Name,
    string? Description,
    IFormFile? File) : IRequest<ErrorOr<Success>>;