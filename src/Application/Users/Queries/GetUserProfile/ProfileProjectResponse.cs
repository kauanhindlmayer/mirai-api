namespace Application.Users.Queries.GetUserProfile;

public sealed record ProfileProjectResponse(
    Guid Id,
    string Name,
    string RoleName);
