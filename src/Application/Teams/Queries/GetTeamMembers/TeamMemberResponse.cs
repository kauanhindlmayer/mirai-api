namespace Application.Teams.Queries.GetTeamMembers;

public sealed record TeamMemberResponse(Guid Id, string Name, Guid RoleId, string RoleName);
