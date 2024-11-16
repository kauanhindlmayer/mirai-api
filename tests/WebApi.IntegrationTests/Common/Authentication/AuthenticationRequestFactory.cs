using Contracts.Users;

namespace WebApi.IntegrationTests.Common.Authentication;

public static class AuthenticationRequestFactory
{
    public static LoginUserRequest CreateLoginRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password)
    {
        return new(email, password);
    }

    public static RegisterUserRequest CreateRegisterRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password,
        string firstName = Constants.User.FirstName,
        string lastName = Constants.User.LastName)
    {
        return new(firstName, lastName, email, password);
    }
}