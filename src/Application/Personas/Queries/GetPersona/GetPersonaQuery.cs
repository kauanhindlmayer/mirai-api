using ErrorOr;
using MediatR;

namespace Application.Personas.Queries.GetPersona;

public sealed record GetPersonaQuery(Guid PersonaId) : IRequest<ErrorOr<PersonaResponse>>;