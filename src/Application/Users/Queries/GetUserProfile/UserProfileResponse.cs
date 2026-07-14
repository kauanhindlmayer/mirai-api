namespace Application.Users.Queries.GetUserProfile;

public sealed record UserProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string? AvatarUrl,
    IReadOnlyList<ProfileProjectResponse> Projects,
    IReadOnlyList<ProfileTeamResponse> Teams);
