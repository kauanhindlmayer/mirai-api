using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UpdateWorkItem;

public sealed record UpdateWorkItemCommand(
    Guid WorkItemId,
    WorkItemType? Type,
    string? Title,
    string? Description,
    string? AcceptanceCriteria,
    WorkItemStatus? Status,
    Guid? AssigneeId,
    Guid? AssignedTeamId,
    Guid? SprintId,
    Guid? ParentWorkItemId,
    Planning? Planning,
    Classification? Classification) : IRequest<ErrorOr<Guid>>;