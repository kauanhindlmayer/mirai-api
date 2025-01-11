using Contracts.Common;

namespace Contracts.WorkItems;

public sealed record CreateWorkItemRequest(
    WorkItemType Type,
    string Title);