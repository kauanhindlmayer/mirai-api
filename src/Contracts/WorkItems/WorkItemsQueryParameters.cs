using Contracts.Common;
using Domain.WorkItems.Enums;

namespace Contracts.WorkItems;

public record WorkItemsQueryParameters(
    WorkItemType? Type,
    WorkItemStatus? Status) : PageRequest;