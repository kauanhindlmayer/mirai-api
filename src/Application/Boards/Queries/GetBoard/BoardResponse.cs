namespace Application.Boards.Queries.GetBoard;

public sealed class BoardResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public IEnumerable<BoardColumnResponse> Columns { get; init; } = [];
}

public sealed class BoardColumnResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int Position { get; init; }
    public int? WipLimit { get; init; }
    public string DefinitionOfDone { get; init; } = null!;
    public IEnumerable<BoardCardResponse> Cards { get; init; } = [];
}

public sealed class BoardCardResponse
{
    public Guid Id { get; init; }
    public int Position { get; init; }
    public WorkItemResponse WorkItem { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class WorkItemResponse
{
    public Guid Id { get; init; }
    public int Code { get; init; }
    public string Title { get; init; } = null!;
    public int? StoryPoints { get; init; }
    public AssigneeResponse? Assignee { get; init; }
    public string Type { get; init; } = null!;
    public string Status { get; init; } = null!;
    public IEnumerable<string> Tags { get; init; } = [];
}

public sealed class AssigneeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
}
