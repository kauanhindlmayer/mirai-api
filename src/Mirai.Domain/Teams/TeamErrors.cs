using ErrorOr;

namespace Mirai.Domain.Teams;

public static class TeamErrors
{
    public static readonly Error TeamNotFound = Error.NotFound(
        code: "Team.TeamNotFound",
        description: "Team not found.");

    public static readonly Error TeamMemberNotFound = Error.NotFound(
        code: "Team.TeamMemberNotFound",
        description: "Team member not found.");

    public static readonly Error TeamMemberAlreadyExists = Error.Validation(
        code: "Team.TeamMemberAlreadyExists",
        description: "User is already a member of this team.");
}