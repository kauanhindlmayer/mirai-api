using ErrorOr;
using MediatR;

namespace Application.Dashboards.Queries.GetDashboard;

public sealed record GetDashboardQuery(
    Guid TeamId,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<ErrorOr<DashboardResponse>>;
