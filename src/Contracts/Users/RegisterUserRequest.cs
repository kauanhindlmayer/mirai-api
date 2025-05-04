namespace Contracts.Users;

/// <summary>
/// Data transfer object for registering a user.
/// </summary>
/// <param name="Email">The email of the user.</param>
/// <param name="Password">The password of the user.</param>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);