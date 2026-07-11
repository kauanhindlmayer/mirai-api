namespace Application.WorkItems.Queries.GetWorkItemHistory;

public sealed class WorkItemChangeSetResponse
{
    public Guid Id { get; init; }
    public WorkItemChangeActorResponse? ChangedBy { get; init; }
    public string? SystemActor { get; init; }
    public IReadOnlyList<WorkItemChangeResponse> Changes { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class WorkItemChangeActorResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}

public sealed class WorkItemChangeResponse
{
    public required string FieldName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}
