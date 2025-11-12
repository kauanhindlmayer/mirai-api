using System.Net.Http.Json;
using Application.Organizations.Queries.GetOrganization;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Organizations;

public class ListOrganizationsTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task ListOrganizations_WhenOrganizationsExist_ShouldReturnOrganizations()
    {
        // Arrange
        var createOrganizationRequest1 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 1");
        var createOrganizationRequest2 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 2");
        var createOrganizationResponse1 = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest1,
            cancellationToken: TestContext.Current.CancellationToken);
        var createOrganizationResponse2 = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest2,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId1 = await createOrganizationResponse1.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId2 = await createOrganizationResponse2.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var listOrganizationsResponse = await fixture.HttpClient.GetAsync(
            Routes.Organizations.List,
            TestContext.Current.CancellationToken);

        // Assert
        listOrganizationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganizations = await listOrganizationsResponse.Content.ReadFromJsonAsync<List<OrganizationResponse>>(
            cancellationToken: TestContext.Current.CancellationToken);
        fetchedOrganizations.Should().NotBeEmpty();
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == organizationId1);
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == organizationId2);
    }
}