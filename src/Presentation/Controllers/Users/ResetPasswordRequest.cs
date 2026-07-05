namespace Presentation.Controllers.Users;

/// <summary>
/// Request to reset a user's password using a reset token.
/// </summary>
/// <param name="Email">The email of the user.</param>
/// <param name="Token">The password reset token.</param>
/// <param name="NewPassword">The new password.</param>
public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);
