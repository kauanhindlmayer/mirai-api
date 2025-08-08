namespace Presentation.Controllers.Organizations;

/// <summary>
/// Request to update an existing organization.
/// </summary>
/// <param name="Name">The name of the organization.</param>
/// <param name="Description">The description of the organization.</param>
public sealed record UpdateOrganizationRequest(
    string Name,
    string Description);