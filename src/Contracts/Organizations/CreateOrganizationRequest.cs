namespace Contracts.Organizations;

public sealed record CreateOrganizationRequest
{
    /// <summary>
    /// The name of the organization.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The description of the organization.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}