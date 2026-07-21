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

    public static Error TeamAlreadyHasActiveSprint(string sprintName) => Error.Conflict(
        code: "Sprint.TeamAlreadyHasActiveSprint",
        description: $"The sprint '{sprintName}' is already active. Complete it before starting another.");
}