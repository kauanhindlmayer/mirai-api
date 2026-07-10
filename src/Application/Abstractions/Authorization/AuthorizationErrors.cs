using ErrorOr;

namespace Application.Abstractions.Authorization;

public static class AuthorizationErrors
{
    public static readonly Error Forbidden = Error.Unauthorized(
        "Authorization.Forbidden",
        "You do not have permission to perform this action.");
}
