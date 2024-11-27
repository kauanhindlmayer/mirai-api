namespace Contracts.Organizations;

public sealed record CreateOrganizationRequest(
    string Name,
    string Description);