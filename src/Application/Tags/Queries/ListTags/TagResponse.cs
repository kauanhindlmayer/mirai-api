namespace Application.Tags.Queries.ListTags;

public sealed class TagResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Color { get; init; }
    public int WorkItemsCount { get; init; }
}