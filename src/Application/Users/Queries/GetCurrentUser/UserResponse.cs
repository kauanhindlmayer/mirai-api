namespace Application.Users.Queries.GetCurrentUser;

public sealed class UserResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string ImageUrl { get; init; }
}