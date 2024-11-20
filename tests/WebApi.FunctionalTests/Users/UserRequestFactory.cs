using Contracts.Users;
using TestCommon.TestConstants;

namespace WebApi.FunctionalTests.Users;

public static class UserRequestFactory
{
    public static LoginUserRequest CreateLoginUserRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password)
    {
        return new LoginUserRequest(email, password);
    }

    public static RegisterUserRequest CreateRegisterUserRequest(
        string email = Constants.User.Email,
        string password = Constants.User.Password,
        string firstName = Constants.User.FirstName,
        string lastName = Constants.User.LastName)
    {
        return new RegisterUserRequest(email, password, firstName, lastName);
    }
}