namespace Application.Users.Queries.GetUserProfile;

public sealed record ProfileTeamResponse(
    Guid Id,
    string Name,
    string ProjectName,
    string RoleName);
