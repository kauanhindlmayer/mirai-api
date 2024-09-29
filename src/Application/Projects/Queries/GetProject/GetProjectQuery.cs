using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId) : IRequest<ErrorOr<Project>>;