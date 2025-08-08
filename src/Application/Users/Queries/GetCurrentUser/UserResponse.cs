using Application.Abstractions;

namespace Application.Users.Queries.GetCurrentUser;

public sealed class UserResponse : LinksResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public string? ImageUrl { get; init; }
}