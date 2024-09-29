using ErrorOr;

namespace Application.Authentication.Common;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = Error.Validation(
        code: "Authentication.InvalidCredentials",
        description: "Invalid credentials");

    public static readonly Error UserAlreadyExists = Error.Conflict(
        code: "Authentication.UserAlreadyExists",
        description: "User already exists");
}