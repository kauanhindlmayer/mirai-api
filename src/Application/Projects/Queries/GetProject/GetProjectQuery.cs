using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IRequest<ErrorOr<ProjectResponse>>;