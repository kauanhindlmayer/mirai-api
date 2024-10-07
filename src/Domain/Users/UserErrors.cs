using ErrorOr;

namespace Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "User.NotFound",
        description: "User not found.");

    public static readonly Error AlreadyExists = Error.Conflict(
        code: "User.AlreadyExists",
        description: "User already exists.");
}