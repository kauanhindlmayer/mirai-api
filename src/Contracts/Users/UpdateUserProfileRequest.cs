namespace Contracts.Users;

public sealed record UpdateUserProfileRequest(
    string FirstName,
    string LastName);