namespace Contracts.Users;

/// <summary>
/// Data transfer object for updating a user's profile.
/// </summary>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
public sealed record UpdateUserProfileRequest(
    string FirstName,
    string LastName);