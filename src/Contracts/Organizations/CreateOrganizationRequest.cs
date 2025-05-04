namespace Contracts.Organizations;

/// <summary>
/// Data transfer object for creating an organization.
/// </summary>
/// <param name="Name">The name of the organization.</param>
/// <param name="Description">The description of the organization.</param>
public sealed record CreateOrganizationRequest(
    string Name,
    string Description);