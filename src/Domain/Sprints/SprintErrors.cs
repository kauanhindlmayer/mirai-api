using ErrorOr;

namespace Domain.Sprints;

public static class SprintErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Sprint.NotFound",
        description: "Sprint not found.");

    public static readonly Error AlreadyExists = Error.Validation(
        code: "Sprint.AlreadyExists",
        description: "Sprint already exists.");

    public static readonly Error WorkItemAlreadyInSprint = Error.Validation(
        code: "Sprint.WorkItemAlreadyInSprint",
        description: "Work item already in sprint.");

    public static readonly Error Overlaps = Error.Validation(
        code: "Sprint.Overlaps",
        description: "Sprint overlaps another sprint in the same team.");

    public static Error OverlapsSprint(string sprintName) => Error.Validation(
        code: "Sprint.Overlaps",
        description: $"These dates overlap the sprint '{sprintName}'.");

    public static Error NameTakenBySprint(string sprintName) => Error.Validation(
        code: "Sprint.AlreadyExists",
        description: $"A sprint named '{sprintName}' already exists in this team.");

    public static readonly Error NotPlanned = Error.Validation(
        code: "Sprint.NotPlanned",
        description: "Only a planned sprint can be started.");

    public static readonly Error NotActive = Error.Validation(
        code: "Sprint.NotActive",
        description: "Only an active sprint can be completed.");

    public static readonly Error DispositionsIncomplete = Error.Validation(
        code: "Sprint.DispositionsIncomplete",
        description: "Every incomplete work item must have a carry-over disposition.");

    public static readonly Error InvalidCarryOverTarget = Error.Validation(
        code: "Sprint.InvalidCarryOverTarget",
        description: "A carry-over target must be a planned sprint on the same team.");

    public static readonly Error IsCompleted = Error.Validation(
        code: "Sprint.IsCompleted",
        description: "A completed sprint is read-only and cannot be changed.");

    public static readonly Error CannotDeleteAfterStart = Error.Validation(
        code: "Sprint.CannotDeleteAfterStart",
        description: "A sprint can only be deleted while it is planned.");

    public static Error TeamAlreadyHasActiveSprint(string sprintName) => Error.Conflict(
        code: "Sprint.TeamAlreadyHasActiveSprint",
        description: $"The sprint '{sprintName}' is already active. Complete it before starting another.");
}