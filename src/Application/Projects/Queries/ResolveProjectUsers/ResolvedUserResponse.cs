namespace Application.Projects.Queries.ResolveProjectUsers;

public sealed record ResolvedUserResponse(
    Guid Id,
    string FullName,
    string? ImageUrl);
