namespace Application.Projects.Queries.GetProject;

public sealed class GitHubRepositoryConnectionResponse
{
    public required long RepositoryId { get; init; }
    public required string RepositoryOwner { get; init; }
    public required string RepositoryName { get; init; }
    public DateTime ConnectedAtUtc { get; init; }
}
