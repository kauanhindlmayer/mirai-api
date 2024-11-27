namespace Contracts.Organizations;

public sealed record UpdateOrganizationRequest(
    string Name,
    string Description);