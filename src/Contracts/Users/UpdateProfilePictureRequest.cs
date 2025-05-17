using Microsoft.AspNetCore.Http;

namespace Contracts.Users;

/// <summary>
/// Data transfer object for updating a user's profile picture.
/// </summary>
/// <param name="File">The profile picture file to upload.</param>
public sealed record UpdateProfilePictureRequest(IFormFile File);