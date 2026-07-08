namespace Application.Projects.GitHub;

/// <summary>
/// Opaque CSRF-protection state stashed in cache between requesting a GitHub
/// App install URL and the installation callback listing that installation's
/// repositories, so the callback can only be completed for the org/project
/// that initiated it.
/// </summary>
public sealed record GitHubInstallationState(Guid OrganizationId, Guid ProjectId, Guid UserId);
