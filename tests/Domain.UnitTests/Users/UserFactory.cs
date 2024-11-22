using Domain.Users;

namespace Domain.UnitTests.Users;

public static class UserFactory
{
    public static User CreateUser(
        string firstName = "John",
        string lastName = "Doe",
        string emailName = "john.doe@email.com")
    {
        return new(firstName, lastName, emailName);
    }
}