namespace Presentation.Controllers.Projects;

/// <summary>
/// Request to resolve users by id within a project's organization.
/// </summary>
public sealed class ResolveProjectUsersRequest
{
    /// <summary>
    /// The unique identifiers of the users to resolve.
    /// </summary>
    public List<Guid> UserIds { get; init; } = [];
}
