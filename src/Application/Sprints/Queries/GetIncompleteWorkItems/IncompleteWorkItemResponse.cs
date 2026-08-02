namespace Application.Sprints.Queries.GetIncompleteWorkItems;

public sealed class IncompleteWorkItemResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
}