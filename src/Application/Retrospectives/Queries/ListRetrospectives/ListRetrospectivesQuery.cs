using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.ListRetrospectives;

public sealed record ListRetrospectivesQuery(Guid TeamId)
    : IRequest<ErrorOr<IReadOnlyList<RetrospectiveBriefResponse>>>;