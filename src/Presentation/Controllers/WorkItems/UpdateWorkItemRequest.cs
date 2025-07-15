using Application.WorkItems.Commands.UpdateWorkItem;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;

namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Data transfer object for updating a work item.
/// </summary>
/// <param name="Type">The type of the work item.</param>
/// <param name="Title">The title of the work item.</param>
/// <param name="Description">The description of the work item.</param>
/// <param name="AcceptanceCriteria">The acceptance criteria for the work item.</param>
/// <param name="Status">The status of the work item.</param>
/// <param name="AssigneeId">The unique identifier of the user assigned to the work item.</param>
/// <param name="AssignedTeamId">The unique identifier of the team assigned to the work item.</param>
/// <param name="SprintId">The unique identifier of the sprint the work item belongs to.</param>
/// <param name="ParentWorkItemId">The unique identifier of the parent work item.</param>
/// <param name="Planning">The planning details for the work item.</param>
/// <param name="Classification">The classification details for the work item.</param>
public sealed record UpdateWorkItemRequest(
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
    Classification? Classification)
{
    public UpdateWorkItemCommand ToCommand(Guid workItemId)
    {
        return new(
            workItemId,
            Type,
            Title,
            Description,
            AcceptanceCriteria,
            Status,
            AssigneeId,
            AssignedTeamId,
            SprintId,
            ParentWorkItemId,
            Planning,
            Classification);
    }
}