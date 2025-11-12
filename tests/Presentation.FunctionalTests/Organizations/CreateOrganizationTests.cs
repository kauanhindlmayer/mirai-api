using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Organizations;

public class CreateOrganizationTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task CreateOrganization_WhenRequestIsValid_ShouldReturnCreatedOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();

        // Act
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        organizationId.Should().NotBeEmpty();
        createOrganizationResponse.Headers.Location.Should().NotBeNull();
        createOrganizationResponse.Headers.Location.AbsolutePath.Should()
            .Be(Routes.Organizations.Get(organizationId));
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: string.Empty);

        // Act
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: new string('a', 256));

        // Act
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenDescriptionIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(description: new string('a', 501));

        // Act
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}