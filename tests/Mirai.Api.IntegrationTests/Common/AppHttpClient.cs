using Mirai.Api.IntegrationTests.Common.Organizations;
using Mirai.Contracts.Organizations;

namespace Mirai.Api.IntegrationTests.Common;

public class AppHttpClient(HttpClient _httpClient)
{
    public async Task<OrganizationResponse> GetOrganizationAndExpectSuccessAsync(Guid id)
    {
        var response = await GetOrganizationAsync(id);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        return organizationResponse!;
    }

    public async Task<HttpResponseMessage> GetOrganizationAsync(Guid id)
    {
        return await _httpClient.GetAsync($"api/organizations/{id}");
    }

    public async Task<OrganizationResponse> CreateOrganizationAndExpectSuccessAsync(
        CreateOrganizationRequest? createOrganizationRequest = null)
    {
        var response = await CreateOrganizationAsync(createOrganizationRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        return organizationResponse!;
    }

    public async Task<HttpResponseMessage> CreateOrganizationAsync(
        CreateOrganizationRequest? createOrganizationRequest = null)
    {
        createOrganizationRequest ??= OrganizationRequestFactory.CreateOrganizationRequest();
        return await _httpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
    }
}