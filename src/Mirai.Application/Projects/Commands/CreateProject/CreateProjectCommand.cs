using ErrorOr;
using MediatR;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string? Description,
    Guid OrganizationId) : IRequest<ErrorOr<Project>>;