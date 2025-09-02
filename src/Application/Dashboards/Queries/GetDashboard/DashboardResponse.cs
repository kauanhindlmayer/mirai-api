namespace Application.Dashboards.Queries.GetDashboard;

public sealed record DashboardResponse(
    List<BurnupPoint> BurnupData,
    List<BurndownPoint> BurndownData,
    List<LeadTimePoint> LeadTimeData,
    List<CycleTimePoint> CycleTimeData,
    List<VelocityPoint> VelocityData,
    DateTime StartDate,
    DateTime EndDate);

public sealed record BurnupPoint(
    DateTime Date,
    int CompletedWork,
    int TotalWork);

public sealed record BurndownPoint(
    DateTime Date,
    int RemainingWork);

public sealed record LeadTimePoint(
    DateTime CompletedDate,
    int LeadTimeDays,
    string WorkItemTitle,
    string WorkItemType);

public sealed record CycleTimePoint(
    DateTime CompletedDate,
    int CycleTimeDays,
    string WorkItemTitle,
    string WorkItemType);

public sealed record VelocityPoint(
    string SprintName,
    DateTime SprintStartDate,
    DateTime SprintEndDate,
    int CompletedStoryPoints,
    int CompletedWorkItems);
