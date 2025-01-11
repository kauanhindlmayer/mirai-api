using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItemsStats;

public sealed record GetWorkItemsStatsQuery(
    Guid ProjectId,
    int PeriodInDays) : IRequest<ErrorOr<WorkItemsStatsResponse>>;