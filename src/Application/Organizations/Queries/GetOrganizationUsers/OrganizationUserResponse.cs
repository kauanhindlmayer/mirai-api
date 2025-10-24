namespace Application.Organizations.Queries.GetOrganizationUsers;

public sealed record OrganizationUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string? ImageUrl,
    DateTime? LastActiveAtUtc);