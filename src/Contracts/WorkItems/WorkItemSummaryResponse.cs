using Contracts.Common;

namespace Contracts.WorkItems;

public sealed record WorkItemSummaryResponse(
    Guid Id,
    string Title,
    WorkItemStatus Status,
    WorkItemType Type);