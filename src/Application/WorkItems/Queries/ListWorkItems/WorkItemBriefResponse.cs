namespace Application.WorkItems.Queries.ListWorkItems;

public sealed class WorkItemBriefResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}