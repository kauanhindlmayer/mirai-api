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

    /// <summary>
    /// Queries exempt from <see cref="IAuthorizationRequest"/> because they have no pre-existing
    /// organization/project/team resource to check the caller against (bootstrap or self-service actions).
    /// </summary>
    private static readonly string[] ExemptQueryNames =
    [
        "GetCurrentUserQuery",
        "GetEffectivePermissionsQuery",
        "GetUserAvatarQuery",
        "ListOrganizationsQuery",
        "ListRolesQuery",
        "LoginUserQuery",
    ];

    [Fact]
    public void Command_ShouldImplement_IAuthorizationRequest_UnlessExempt()
    {
        var result = GetAuthorizationResult("Command", ExemptCommandNames);

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(result, "commands", nameof(ExemptCommandNames)));
    }

    [Fact]
    public void Query_ShouldImplement_IAuthorizationRequest_UnlessExempt()
    {
        var result = GetAuthorizationResult("Query", ExemptQueryNames);

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(result, "queries", nameof(ExemptQueryNames)));
    }

    private static TestResult GetAuthorizationResult(string nameSuffix, string[] exemptNames)
    {
        return Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith(nameSuffix)
            .And()
            .AreNotInterfaces()
            .And()
            .DoNotHaveNameMatching(string.Join('|', exemptNames))
            .Should()
            .ImplementInterface(typeof(IAuthorizationRequest))
            .GetResult();
    }

    private static string BuildFailureMessage(TestResult result, string requestKind, string exemptListName)
    {
        var typeNames = result.FailingTypes?.Select(t => t.Name) ?? [];
        return $"The following {requestKind} must implement IAuthorizationRequest, or be added to " +
               $"{exemptListName} if they genuinely have no resource to authorize against: " +
               string.Join(", ", typeNames);
    }
}
