namespace Contracts.Users;

public sealed record RegisterUserRequest
{
    /// <summary>
    /// The email of the user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The password of the user.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public required string LastName { get; init; }
}