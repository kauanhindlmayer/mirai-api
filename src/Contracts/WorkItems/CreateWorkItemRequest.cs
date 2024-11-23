using Contracts.Common;

namespace Contracts.WorkItems;

public sealed record CreateWorkItemRequest(
    Guid ProjectId,
    WorkItemType Type,
    string Title);