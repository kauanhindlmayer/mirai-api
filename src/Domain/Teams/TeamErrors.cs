using ErrorOr;

namespace Domain.Teams;

public static class TeamErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Team.NotFound",
        description: "Team not found.");

    public static readonly Error MemberNotFound = Error.NotFound(
        code: "Team.MemberNotFound",
        description: "Team member not found.");

    public static readonly Error MemberAlreadyExists = Error.Validation(
        code: "Team.MemberAlreadyExists",
        description: "User is already a member of this team.");
}