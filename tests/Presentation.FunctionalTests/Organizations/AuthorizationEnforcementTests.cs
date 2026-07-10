using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Presentation.Controllers.Organizations;
using Presentation.FunctionalTests.Infrastructure;
using Presentation.FunctionalTests.Users;

namespace Presentation.FunctionalTests.Organizations;

public class AuthorizationEnforcementTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task DeleteOrganization_WhenCallerIsNotOwner_ShouldReturnForbidden()
    {
        // Arrange
        var organizationId = await CreateOrganizationAsOwnerAsync();
        await AddSecondaryUserAsMemberAsync(organizationId);

        // Act
        var response = await fixture.SecondaryHttpClient.DeleteAsync(
            Routes.Organizations.Delete(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteOrganization_WhenCallerIsOwner_ShouldSucceed()
    {
        // Arrange
        var organizationId = await CreateOrganizationAsOwnerAsync();

        // Act
        var response = await fixture.HttpClient.DeleteAsync(
            Routes.Organizations.Delete(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateOrganization_WhenCallerIsNotOwner_ShouldReturnForbidden()
    {
        // Arrange
        var organizationId = await CreateOrganizationAsOwnerAsync();
        await AddSecondaryUserAsMemberAsync(organizationId);
        var updateRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var response = await fixture.SecondaryHttpClient.PutAsJsonAsync(
            Routes.Organizations.Update(organizationId),
            updateRequest,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganizationAsOwnerAsync()
    {
        var createRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createRequest,
            TestContext.Current.CancellationToken);

        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    private async Task AddSecondaryUserAsMemberAsync(Guid organizationId)
    {
        var addUserRequest = new AddUserToOrganizationRequest(UserRequestFactory.SecondaryEmail);
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.AddUser(organizationId),
            addUserRequest,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
