using ErrorOr;
using MediatR;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Queries.ListProjects;

public record ListProjectsQuery(Guid OrganizationId) : IRequest<ErrorOr<List<Project>>>;