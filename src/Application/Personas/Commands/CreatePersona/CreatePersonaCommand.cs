using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Personas.Commands.CreatePersona;

public sealed record CreatePersonaCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    IFormFile? File) : IRequest<ErrorOr<Guid>>;