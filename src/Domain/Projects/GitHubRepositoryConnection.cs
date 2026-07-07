using Domain.Shared;

namespace Domain.Projects;

/// <summary>
/// Represents a project's connection to a single GitHub repository, used to
/// automatically and manually link pull requests to the project's work items.
/// </summary>
public sealed class GitHubRepositoryConnection : Entity
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public long InstallationId { get; private set; }
    public long RepositoryId { get; private set; }
    public string RepositoryOwner { get; private set; } = null!;
    public string RepositoryName { get; private set; } = null!;
    public Guid ConnectedByUserId { get; private set; }
    public DateTime ConnectedAtUtc { get; private set; }
    public DateTime? LastSyncedAtUtc { get; private set; }

    public GitHubRepositoryConnection(
        Guid projectId,
        long installationId,
        long repositoryId,
        string repositoryOwner,
        string repositoryName,
        Guid connectedByUserId)
    {
        ProjectId = projectId;
        InstallationId = installationId;
        RepositoryId = repositoryId;
        RepositoryOwner = repositoryOwner;
        RepositoryName = repositoryName;
        ConnectedByUserId = connectedByUserId;
        ConnectedAtUtc = DateTime.UtcNow;
    }

    private GitHubRepositoryConnection()
    {
    }

    public void UpdateLastSyncedAt(DateTime timestampUtc)
    {
        LastSyncedAtUtc = timestampUtc;
    }
}
