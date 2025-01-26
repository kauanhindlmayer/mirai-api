using ErrorOr;
using MediatR;

namespace Application.Sprints.Queries.ListSprints;

public sealed record ListSprintsQuery(Guid TeamId) : IRequest<ErrorOr<IReadOnlyList<SprintResponse>>>;