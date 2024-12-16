using Contracts.Common;

namespace Contracts.WorkItems;

public sealed record WorkItemSummaryResponse(
    Guid Id,
    int Code,
    string Title,
    WorkItemStatus Status,
    WorkItemType Type,
    DateTime UpdatedAt);