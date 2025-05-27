using ErrorOr;
using MediatR;

namespace Application.Personas.Queries.ListPersonas;

public sealed record ListPersonasQuery(Guid ProjectId)
    : IRequest<ErrorOr<IReadOnlyList<PersonaBriefResponse>>>;