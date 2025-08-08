namespace Presentation.Controllers.Users;

/// <summary>
/// Request to update the profile picture of a user.
/// </summary>
/// <param name="File">The profile picture file to upload.</param>
public sealed record UpdateProfilePictureRequest(IFormFile File);