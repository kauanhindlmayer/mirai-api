namespace Presentation.Controllers.Organizations;

/// <summary>
/// Request to create a new organization.
/// </summary>
/// <param name="Name">The name of the organization.</param>
/// <param name="Description">The description of the organization.</param>
public sealed record CreateOrganizationRequest(
    string Name,
    string Description);