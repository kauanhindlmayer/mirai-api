using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.ListRetrospectives;

public record ListRetrospectivesQuery(Guid TeamId) : IRequest<ErrorOr<List<Retrospective>>>;