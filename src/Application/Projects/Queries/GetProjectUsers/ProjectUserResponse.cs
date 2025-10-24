namespace Application.Projects.Queries.GetProjectUsers;

public sealed record ProjectUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string? ImageUrl);