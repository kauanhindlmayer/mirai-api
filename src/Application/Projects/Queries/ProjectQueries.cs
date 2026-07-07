using System.Linq.Expressions;
using Application.Projects.Queries.GetProject;
using Domain.Projects;

namespace Application.Projects.Queries;

internal static class ProjectQueries
{
    public static Expression<Func<Project, ProjectResponse>> ProjectToDto()
    {
        return p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            OrganizationId = p.OrganizationId,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            GitHubRepositoryConnection = p.GitHubRepositoryConnection == null
                ? null
                : new GitHubRepositoryConnectionResponse
                {
                    RepositoryId = p.GitHubRepositoryConnection.RepositoryId,
                    RepositoryOwner = p.GitHubRepositoryConnection.RepositoryOwner,
                    RepositoryName = p.GitHubRepositoryConnection.RepositoryName,
                    ConnectedAtUtc = p.GitHubRepositoryConnection.ConnectedAtUtc,
                },
        };
    }
}