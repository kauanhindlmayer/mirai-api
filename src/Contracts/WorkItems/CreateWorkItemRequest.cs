using Contracts.Common;

namespace Contracts.WorkItems;

public record CreateWorkItemRequest(
    Guid ProjectId,
    WorkItemType Type,
    string Title);