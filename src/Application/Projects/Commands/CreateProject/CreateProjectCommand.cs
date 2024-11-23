using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string Description,
    Guid OrganizationId) : IRequest<ErrorOr<Project>>;