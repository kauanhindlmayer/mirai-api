using Domain.Backlogs;
using ErrorOr;
using MediatR;

namespace Application.Backlogs.Queries.GetBacklog;

public sealed record GetBacklogQuery(
    Guid TeamId,
    Guid? SprintId,
    BacklogLevel? BacklogLevel) : IRequest<ErrorOr<IReadOnlyList<BacklogResponse>>>;