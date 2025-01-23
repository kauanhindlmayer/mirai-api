using Domain.Backlogs;
using ErrorOr;
using MediatR;

namespace Application.Backlogs.Queries.GetBacklog;

public sealed record GetBacklogQuery(
    Guid TeamId,
    BacklogLevel? BacklogLevel) : IRequest<ErrorOr<IReadOnlyList<BacklogResponse>>>;