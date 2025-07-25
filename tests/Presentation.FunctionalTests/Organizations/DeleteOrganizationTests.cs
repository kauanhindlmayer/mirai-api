using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Organizations;

public class DeleteOrganizationTests : BaseFunctionalTest
{
    public DeleteOrganizationTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationExists_ShouldDeleteOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var deleteOrganizationResponse = await _httpClient.DeleteAsync(
            Routes.Organizations.Delete(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        deleteOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();

        // Act
        var deleteOrganizationResponse = await _httpClient.DeleteAsync(
            Routes.Organizations.Delete(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        deleteOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}