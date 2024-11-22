using Contracts.Users;

namespace WebApi.FunctionalTests.Users;

public static class UserRequestFactory
{
    public static LoginUserRequest CreateLoginUserRequest(
        string email = "john.doe@email.com",
        string password = "vXJu9zCgjOV2dW3")
    {
        return new LoginUserRequest(email, password);
    }

    public static RegisterUserRequest CreateRegisterUserRequest(
        string email = "john.doe@email.com",
        string password = "vXJu9zCgjOV2dW3",
        string firstName = "John",
        string lastName = "Doe")
    {
        return new RegisterUserRequest(email, password, firstName, lastName);
    }
}