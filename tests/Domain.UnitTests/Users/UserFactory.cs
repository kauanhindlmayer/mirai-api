using Domain.Users;

namespace Domain.UnitTests.Users;

public static class UserFactory
{
    public const string FirstName = "John";
    public const string LastName = "Doe";
    public const string FullName = "John Doe";
    public const string Email = "john.doe@email.com";
    public const string IdentityId = "identity_id";
    public const string ImageUrl = "image_url";
    public static readonly Guid ImageFileId = Guid.NewGuid();

    public static User CreateUser(
        string firstName = FirstName,
        string lastName = LastName,
        string emailName = Email)
    {
        return new(firstName, lastName, emailName);
    }
}