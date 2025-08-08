using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.AddUserToProject;

public sealed record AddUserToProjectCommand(
    Guid ProjectId,
    Guid UserId) : IRequest<ErrorOr<Success>>;