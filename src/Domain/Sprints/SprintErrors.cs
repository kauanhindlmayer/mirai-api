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
}