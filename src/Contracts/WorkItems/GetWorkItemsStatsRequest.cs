namespace Contracts.WorkItems;

public sealed record GetWorkItemsStatsRequest
{
    /// <summary>
    /// The period in days to get the work items stats for.
    /// </summary>
    public int PeriodInDays { get; init; }
}