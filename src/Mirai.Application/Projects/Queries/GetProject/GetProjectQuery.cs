using ErrorOr;
using MediatR;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId) : IRequest<ErrorOr<Project>>;