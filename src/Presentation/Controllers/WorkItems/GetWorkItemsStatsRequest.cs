namespace Presentation.Controllers.WorkItems;

/// <summary>
/// /// Data transfer object for getting work items stats.
/// </summary>
/// <param name="PeriodInDays">The period in days to get the work items stats for.</param>
public sealed record GetWorkItemsStatsRequest(int PeriodInDays);