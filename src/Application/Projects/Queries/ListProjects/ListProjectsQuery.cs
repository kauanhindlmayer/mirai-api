using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(Guid OrganizationId)
    : IRequest<ErrorOr<List<Project>>>;