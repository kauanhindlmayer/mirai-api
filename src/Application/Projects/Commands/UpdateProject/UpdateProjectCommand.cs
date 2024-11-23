using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid OrganizationId,
    Guid ProjectId,
    string Name,
    string Description) : IRequest<ErrorOr<Project>>;