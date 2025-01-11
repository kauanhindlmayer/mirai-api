using Application.Projects.Queries.GetProject;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(Guid OrganizationId)
    : IRequest<ErrorOr<IReadOnlyList<ProjectResponse>>>;