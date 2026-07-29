namespace Domain.Sprints;

/// <summary>
/// Where an incomplete work item should go when its sprint is completed. The
/// target is an explicit discriminator so that "send to the backlog" is never
/// confused with "no decision was made".
/// </summary>
public sealed record SprintItemDisposition(
    Guid WorkItemId,
    DispositionTarget Target,
    Guid? TargetSprintId);
