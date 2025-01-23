using Domain.WorkItems.Enums;

namespace Contracts.WorkItems;

public sealed record CreateWorkItemRequest(
    WorkItemType Type,
    string Title,
    Guid? AssignedTeamId);