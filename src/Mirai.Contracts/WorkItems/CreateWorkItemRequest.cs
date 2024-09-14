namespace Mirai.Contracts.WorkItems;

public record CreateWorkItemRequest(Guid ProjectId, string Type, string Title);