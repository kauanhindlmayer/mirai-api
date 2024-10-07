using ErrorOr;

namespace Domain.Users;

public static class UserErrors
{
    public static readonly Error UserNotFound = Error.NotFound(
        code: "User.UserNotFound",
        description: "User not found.");

    public static readonly Error UserAlreadyExists = Error.Conflict(
        code: "User.UserAlreadyExists",
        description: "User already exists.");
}