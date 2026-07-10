using Application.Abstractions.Authorization;
using ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace ArchitectureTests.Application;

public class AuthorizationTests : BaseTest
{
    /// <summary>
    /// Commands exempt from <see cref="IAuthorizationRequest"/> because they have no pre-existing
    /// organization/project/team resource to check the caller against (bootstrap or self-service actions).
    /// </summary>
    private static readonly string[] ExemptCommandNames =
    [
        "CreateOrganizationCommand",
        "ForgotPasswordCommand",
        "LoginWithGitHubCommand",
        "RegisterUserCommand",
        "ResetPasswordCommand",
        "UpdateUserAvatarCommand",
        "UpdateUserProfileCommand",
    ];

    [Fact]
    public void Command_ShouldImplement_IAuthorizationRequest_UnlessExempt()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .DoNotHaveNameMatching(string.Join('|', ExemptCommandNames))
            .Should()
            .ImplementInterface(typeof(IAuthorizationRequest))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(result));
    }

    private static string BuildFailureMessage(TestResult result)
    {
        var typeNames = result.FailingTypes?.Select(t => t.Name) ?? [];
        return "The following commands must implement IAuthorizationRequest, or be added to " +
               $"{nameof(ExemptCommandNames)} if they genuinely have no resource to authorize against: " +
               string.Join(", ", typeNames);
    }
}
