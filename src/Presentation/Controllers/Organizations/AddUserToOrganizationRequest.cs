namespace Presentation.Controllers.Organizations;

/// <summary>
/// Data transfer object for adding a user to an organization.
/// </summary>
/// <param name="UserId">The unique identifier of the user to add.</param>
public sealed record AddUserToOrganizationRequest(Guid UserId);