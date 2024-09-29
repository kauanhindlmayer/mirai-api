using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.ListProjects;

public record ListProjectsQuery(Guid OrganizationId)
    : IRequest<ErrorOr<List<Project>>>;