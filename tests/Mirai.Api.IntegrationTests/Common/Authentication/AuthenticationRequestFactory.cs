using Mirai.Contracts.Authentication;

namespace Mirai.Api.IntegrationTests.Common.Authentication;

public static class AuthenticationRequestFactory
{
    public static LoginRequest CreateLoginRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password)
    {
        return new(email, password);
    }

    public static RegisterRequest CreateRegisterRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password,
        string firstName = Constants.User.FirstName,
        string lastName = Constants.User.LastName)
    {
        return new(firstName, lastName, email, password);
    }
}