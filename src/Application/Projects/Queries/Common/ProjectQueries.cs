using System.Linq.Expressions;
using Application.Projects.Queries.GetProject;
using Domain.Projects;

namespace Application.Projects.Queries.Common;

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
        };
    }
}