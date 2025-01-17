namespace Application.Tags.Queries.ListTags;

public sealed class TagResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public int WorkItemsCount { get; init; }
}