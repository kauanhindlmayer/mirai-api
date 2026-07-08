using Domain.Projects;

namespace Domain.UnitTests.Projects;

public static class GitHubRepositoryConnectionFactory
{
    public const long InstallationId = 1001;
    public const long RepositoryId = 2002;
    public const string RepositoryOwner = "mirai-org";
    public const string RepositoryName = "mirai-app";
    public static readonly Guid ProjectId = Guid.NewGuid();
    public static readonly Guid ConnectedByUserId = Guid.NewGuid();

    public static GitHubRepositoryConnection Create(
        Guid? projectId = null,
        long? installationId = null,
        long? repositoryId = null,
        string? repositoryOwner = null,
        string? repositoryName = null,
        Guid? connectedByUserId = null)
    {
        return new GitHubRepositoryConnection(
            projectId ?? ProjectId,
            installationId ?? InstallationId,
            repositoryId ?? RepositoryId,
            repositoryOwner ?? RepositoryOwner,
            repositoryName ?? RepositoryName,
            connectedByUserId ?? ConnectedByUserId);
    }
}
