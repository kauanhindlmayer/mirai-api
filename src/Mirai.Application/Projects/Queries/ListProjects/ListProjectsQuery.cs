using ErrorOr;
using MediatR;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Queries.ListProjects;

public record ListProjectsQuery() : IRequest<ErrorOr<List<Project>>>;