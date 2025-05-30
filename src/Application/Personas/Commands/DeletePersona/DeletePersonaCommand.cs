using ErrorOr;
using MediatR;

namespace Application.Personas.Commands.DeletePersona;

public sealed record DeletePersonaCommand(
    Guid ProjectId,
    Guid PersonaId) : IRequest<ErrorOr<Success>>;