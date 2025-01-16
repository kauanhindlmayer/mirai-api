using ErrorOr;
using MediatR;

namespace Application.Dashboards.Queries.GetDashboard;

public sealed record GetDashboardQuery(
    Guid ProjectId,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<ErrorOr<DashboardResponse>>;
