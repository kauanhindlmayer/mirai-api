namespace Presentation.Controllers.Users;

/// <summary>
/// Request to update the avatar of a user.
/// </summary>
/// <param name="File">The avatar file to upload.</param>
public sealed record UpdateAvatarRequest(IFormFile File);