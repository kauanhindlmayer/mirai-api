using Domain.Users;

namespace Domain.UnitTests.Users;

internal static class UserData
{
    public const string FirstName = "John";
    public const string LastName = "Doe";
    public const string Email = "john.doe@example.com";

    public static User Create(
        string? firstName = null,
        string? lastName = null,
        string? email = null)
    {
        return new User(
            firstName ?? FirstName,
            lastName ?? LastName,
            email ?? Email);
    }
}