using ErrorOr;

namespace Domain.Authorization;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Role.NotFound",
        "Role not found.");

    public static readonly Error ScopeMismatch = Error.Validation(
        "Role.ScopeMismatch",
        "The role's scope does not match the expected scope.");
}
