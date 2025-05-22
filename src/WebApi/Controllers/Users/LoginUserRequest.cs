namespace WebApi.Controllers.Users;

/// <summary>
/// Data transfer object for logging in a user.
/// </summary>
/// <param name="Email">The email of the user.</param>
/// <param name="Password">The password of the user.</param>
public sealed record LoginUserRequest(
    string Email,
    string Password);