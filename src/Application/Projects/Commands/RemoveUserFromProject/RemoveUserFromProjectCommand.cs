using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.RemoveUserFromProject;

public sealed record RemoveUserFromProjectCommand(
    Guid ProjectId,
    Guid UserId) : IRequest<ErrorOr<Success>>;