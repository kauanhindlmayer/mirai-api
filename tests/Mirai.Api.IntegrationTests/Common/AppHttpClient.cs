using System.Net.Http.Headers;
using Mirai.Api.IntegrationTests.Common.Authentication;
using Mirai.Api.IntegrationTests.Common.Organizations;
using Mirai.Api.IntegrationTests.Common.Projects;
using Mirai.Contracts.Authentication;
using Mirai.Contracts.Organizations;
using Mirai.Contracts.Projects;

namespace Mirai.Api.IntegrationTests.Common;

public class AppHttpClient(HttpClient _httpClient)
{
    public async Task<ProjectResponse> CreateProjectAndExpectSuccessAsync(
        CreateProjectRequest? createProjectRequest = null,
        string? token = null)
    {
        var response = await CreateProjectAsync(createProjectRequest, token);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.AbsolutePath.Should().Be($"/api/projects/{project!.Id}");
        project.Should().NotBeNull();
        return project!;
    }

    public async Task<HttpResponseMessage> CreateProjectAsync(
        CreateProjectRequest? createProjectRequest = null,
        string? token = null)
    {
        createProjectRequest ??= ProjectRequestFactory.CreateCreateProjectRequest();
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.PostAsJsonAsync($"api/projects", createProjectRequest);
    }

    public async Task<OrganizationResponse> GetOrganizationAndExpectSuccessAsync(Guid id, string? token = null)
    {
        var response = await GetOrganizationAsync(id, token);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        return organizationResponse!;
    }

    public async Task<HttpResponseMessage> GetOrganizationAsync(Guid id, string? token = null)
    {
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.GetAsync($"api/organizations/{id}");
    }

    public async Task<OrganizationResponse> CreateOrganizationAndExpectSuccessAsync(
        CreateOrganizationRequest? createOrganizationRequest = null,
        string? token = null)
    {
        createOrganizationRequest ??= OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var response = await CreateOrganizationAsync(createOrganizationRequest, token);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        return organizationResponse!;
    }

    public async Task<HttpResponseMessage> CreateOrganizationAsync(
        CreateOrganizationRequest? createOrganizationRequest = null,
        string? token = null)
    {
        createOrganizationRequest ??= OrganizationRequestFactory.CreateCreateOrganizationRequest();
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
    }

    public async Task<OrganizationResponse> UpdateOrganizationAndExpectSuccessAsync(
        Guid id,
        UpdateOrganizationRequest? updateOrganizationRequest = null,
        string? token = null)
    {
        var response = await UpdateOrganizationAsync(id, updateOrganizationRequest, token);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        return organizationResponse!;
    }

    public async Task<HttpResponseMessage> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequest? updateOrganizationRequest, string? token = null)
    {
        updateOrganizationRequest ??= OrganizationRequestFactory.CreateUpdateOrganizationRequest();
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PutAsJsonAsync($"api/organizations/{id}", updateOrganizationRequest);
        return response;
    }

    public async Task<List<OrganizationResponse>> ListOrganizationsAndExpectSuccessAsync(string? token = null)
    {
        var response = await ListOrganizationsAsync(token);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var organizations = await response.Content.ReadFromJsonAsync<List<OrganizationResponse>>();
        organizations.Should().NotBeNull();
        return organizations!;
    }

    public async Task<HttpResponseMessage> ListOrganizationsAsync(string? token = null)
    {
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.GetAsync("api/organizations");
    }

    public async Task<HttpResponseMessage> DeleteOrganizationAsync(Guid id, string? token = null)
    {
        token ??= await GenerateTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.DeleteAsync($"api/organizations/{id}");
    }

    public async Task<string> GenerateTokenAsync(RegisterRequest? registerRequest = null)
    {
        registerRequest ??= AuthenticationRequestFactory.CreateRegisterRequest();
        var response = await _httpClient.PostAsJsonAsync("api/register", registerRequest);
        response.Should().BeSuccessful();
        var tokenResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        tokenResponse.Should().NotBeNull();
        return tokenResponse!.Token;
    }
}