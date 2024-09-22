using ErrorOr;
using MediatR;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Queries.GetRetrospective;

public record GetRetrospectiveQuery(Guid RetrospectiveId)
    : IRequest<ErrorOr<Retrospective>>;