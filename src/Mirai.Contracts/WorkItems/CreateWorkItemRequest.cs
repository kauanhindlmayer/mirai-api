using Mirai.Contracts.Common;

namespace Mirai.Contracts.WorkItems;

public record CreateWorkItemRequest(
    Guid ProjectId,
    WorkItemType Type,
    string Title);