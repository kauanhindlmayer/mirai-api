using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string Description,
    Guid OrganizationId) : IRequest<ErrorOr<Guid>>;