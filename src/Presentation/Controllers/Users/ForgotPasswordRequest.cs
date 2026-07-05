namespace Presentation.Controllers.Users;

/// <summary>
/// Request to initiate a password reset.
/// </summary>
/// <param name="Email">The email of the user.</param>
public sealed record ForgotPasswordRequest(string Email);
