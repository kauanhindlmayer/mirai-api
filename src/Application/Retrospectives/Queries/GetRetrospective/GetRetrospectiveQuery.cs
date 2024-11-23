using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.GetRetrospective;

public sealed record GetRetrospectiveQuery(Guid RetrospectiveId)
    : IRequest<ErrorOr<Retrospective>>;