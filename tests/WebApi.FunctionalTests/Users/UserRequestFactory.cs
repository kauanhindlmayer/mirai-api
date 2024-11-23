using Contracts.Users;

namespace WebApi.FunctionalTests.Users;

public static class UserRequestFactory
{
    public const string Email = "john.doe@email.com";
    public const string Password = "vXJu9zCgjOV2dW3";
    public const string FirstName = "John";
    public const string LastName = "Doe";

    public static LoginUserRequest CreateLoginUserRequest(
        string email = Email,
        string password = Password)
    {
        return new LoginUserRequest(email, password);
    }

    public static RegisterUserRequest CreateRegisterUserRequest(
        string email = Email,
        string password = Password,
        string firstName = FirstName,
        string lastName = LastName)
    {
        return new RegisterUserRequest(email, password, firstName, lastName);
    }
}