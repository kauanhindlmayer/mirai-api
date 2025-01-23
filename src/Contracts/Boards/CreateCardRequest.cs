using Domain.WorkItems.Enums;

namespace Contracts.Boards;

public sealed record CreateCardRequest(
    WorkItemType Type,
    string Title);