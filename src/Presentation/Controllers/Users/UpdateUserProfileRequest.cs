namespace Presentation.Controllers.Users;

/// <summary>
/// Request to update a user's profile.
/// </summary>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
public sealed record UpdateUserProfileRequest(
    string FirstName,
    string LastName);