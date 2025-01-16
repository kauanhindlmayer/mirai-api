namespace Application.Dashboards.Queries.GetDashboard;

public sealed record DashboardResponse(
    List<BurnupPoint> BurnupData,
    List<BurndownPoint> BurndownData,
    DateTime StartDate,
    DateTime EndDate);

public sealed record BurnupPoint(
    DateTime Date,
    int CompletedWork,
    int TotalWork);

public sealed record BurndownPoint(
    DateTime Date,
    int RemainingWork);
