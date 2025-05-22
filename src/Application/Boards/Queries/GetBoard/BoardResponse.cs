namespace Application.Boards.Queries.GetBoard;

public sealed class BoardResponse
{
    public Guid Id { get; init; }
    public Guid TeamId { get; init; }
    public required string Name { get; init; }
    public IEnumerable<BoardColumnResponse> Columns { get; init; } = [];
}

public sealed class BoardColumnResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public int Position { get; init; }
    public int? WipLimit { get; init; }
    public string? DefinitionOfDone { get; init; }
    public IEnumerable<BoardCardResponse> Cards { get; init; } = [];
}

public sealed class BoardCardResponse
{
    public Guid Id { get; init; }
    public Guid ColumnId { get; init; }
    public int Position { get; init; }
    public WorkItemResponse WorkItem { get; init; } = null!;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class WorkItemResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public required string Title { get; init; }
    public int? StoryPoints { get; init; }
    public AssigneeResponse? Assignee { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public IEnumerable<string> Tags { get; init; } = [];
}

public sealed class AssigneeResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
