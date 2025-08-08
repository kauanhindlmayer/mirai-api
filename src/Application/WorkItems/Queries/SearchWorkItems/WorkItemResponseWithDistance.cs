namespace Application.WorkItems.Queries.SearchWorkItems;

public sealed class WorkItemResponseWithDistance
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Type { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public double? Distance { get; init; }
}