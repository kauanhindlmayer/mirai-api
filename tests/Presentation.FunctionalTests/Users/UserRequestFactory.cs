using Presentation.Controllers.Users;

namespace Presentation.FunctionalTests.Users;

public static class UserRequestFactory
{
    public const string Email = "john.doe@mirai.com";
    public const string Password = "vXJu9zCgjOV2dW3";
    public const string FirstName = "John";
    public const string LastName = "Doe";

    public static LoginUserRequest CreateLoginUserRequest(
        string? email = null,
        string? password = null)
    {
        return new LoginUserRequest(
            email ?? Email,
            password ?? Password);
    }

    public static RegisterUserRequest CreateRegisterUserRequest(
        string? email = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null)
    {
        return new RegisterUserRequest(
            email ?? Email,
            password ?? Password,
            firstName ?? FirstName,
            lastName ?? LastName);
    }
}