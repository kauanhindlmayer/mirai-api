namespace WebApi.Controllers.Dashboards;

/// <summary>
/// Data transfer object for retrieving a dashboard.
/// </summary>
/// <param name="StartDate">
/// The start date for filtering. If not provided, the default is the current date.
/// </param>
/// <param name="EndDate">
/// The end date for filtering. If not provided, the default is the current date plus 14 days.
/// </param>
public sealed record GetDashboardRequest(
    DateTime StartDate,
    DateTime EndDate);