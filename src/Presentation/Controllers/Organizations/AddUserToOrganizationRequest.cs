namespace Presentation.Controllers.Organizations;

/// <summary>
/// Request to add a user to an organization.
/// </summary>
/// <param name="Email">The email address of the user to add.</param>
public sealed record AddUserToOrganizationRequest(string Email);