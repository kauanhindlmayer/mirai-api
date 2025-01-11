namespace Application.WorkItems.Queries.GetWorkItemsStats;

public sealed class WorkItemsStatsResponse
{
    public int WorkItemsCreated { get; init; }
    public int WorkItemsCompleted { get; init; }
}