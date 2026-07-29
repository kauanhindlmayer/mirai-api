using Domain.Sprints;

namespace Presentation.Controllers.Sprints;

/// <summary>
/// Request to complete a sprint, carrying over its incomplete work items.
/// </summary>
/// <param name="Dispositions">
/// Where each incomplete work item should go. Every incomplete item in the
/// sprint must appear exactly once.
/// </param>
public sealed record CompleteSprintRequest(
    IReadOnlyList<SprintItemDispositionRequest> Dispositions);

/// <summary>
/// The carry-over decision for a single incomplete work item.
/// </summary>
/// <param name="WorkItemId">The incomplete work item's unique identifier.</param>
/// <param name="Target">
/// <c>Backlog</c> to return the item to the backlog, or <c>Sprint</c> to roll it
/// forward into a planned sprint named by <paramref name="TargetSprintId"/>.
/// </param>
/// <param name="TargetSprintId">
/// The planned sprint to roll the item into; required when <paramref name="Target"/>
/// is <c>Sprint</c>.
/// </param>
public sealed record SprintItemDispositionRequest(
    Guid WorkItemId,
    DispositionTarget Target,
    Guid? TargetSprintId);
