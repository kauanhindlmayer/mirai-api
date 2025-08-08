namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to get statistics for work items.
/// </summary>
/// <param name="PeriodInDays">The period in days to get the work items stats for.</param>
public sealed record GetWorkItemsStatsRequest(int PeriodInDays);