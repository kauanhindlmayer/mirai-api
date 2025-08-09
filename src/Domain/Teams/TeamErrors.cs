using ErrorOr;

namespace Domain.Teams;

public static class TeamErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Team.NotFound",
        description: "Team not found.");

    public static Error UserAlreadyExists => Error.Conflict(
        code: "Team.UserAlreadyExists",
        description: "User is already a member of this team.");

    public static Error UserNotFound => Error.NotFound(
        code: "Team.UserNotFound",
        description: "User is not a member of this team.");

    public static Error UserHasAssignedWorkItems => Error.Conflict(
        code: "Team.UserHasAssignedWorkItems",
        description: "Cannot remove user who has assigned work items in this team.");

    public static readonly Error BoardAlreadyExists = Error.Conflict(
        code: "Team.BoardAlreadyExists",
        description: "A board already exists for this team.");
}