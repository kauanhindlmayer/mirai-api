namespace WebApi.Controllers.Organizations;

/// <summary>
/// Data transfer object for updating an organization.
/// </summary>
/// <param name="Name">The name of the organization.</param>
/// <param name="Description">The description of the organization.</param>
public sealed record UpdateOrganizationRequest(
    string Name,
    string Description);