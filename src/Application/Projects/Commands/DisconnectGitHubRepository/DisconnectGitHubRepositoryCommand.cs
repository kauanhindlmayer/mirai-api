using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.DisconnectGitHubRepository;

public sealed record DisconnectGitHubRepositoryCommand(
    Guid OrganizationId,
    Guid ProjectId) : IRequest<ErrorOr<Success>>;
