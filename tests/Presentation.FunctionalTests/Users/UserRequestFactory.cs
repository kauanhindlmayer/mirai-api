using Presentation.Controllers.Users;

namespace Presentation.FunctionalTests.Users;

public static class UserRequestFactory
{
    public const string Email = "john.doe@mirai.com";
    public const string Password = "Test@123";
    public const string FirstName = "John";
    public const string LastName = "Doe";

    /// <summary>
    /// A seeded user who is a Member/Contributor (not Owner/Admin) of the default seeded
    /// organization, project, and team - used to exercise authorization-denied paths.
    /// </summary>
    public const string SecondaryEmail = "jane.smith@mirai.com";

    public static LoginUserRequest CreateLoginUserRequest(
        string? email = null,
        string? password = null)
    {
        return new LoginUserRequest(
            email ?? Email,
            password ?? Password);
    }

    public static LoginWithGitHubRequest CreateLoginWithGitHubRequest(
        string? code = null,
        string? redirectUri = null)
    {
        return new LoginWithGitHubRequest(
            code ?? "invalid-code",
            redirectUri ?? "https://localhost:5173/auth/github/callback");
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

    public static ForgotPasswordRequest CreateForgotPasswordRequest(string? email = null)
    {
        return new ForgotPasswordRequest(email ?? Email);
    }

    public static ResetPasswordRequest CreateResetPasswordRequest(
        string? email = null,
        string? token = null,
        string? newPassword = null)
    {
        return new ResetPasswordRequest(
            email ?? Email,
            token ?? "invalid-token",
            newPassword ?? Password);
    }
}