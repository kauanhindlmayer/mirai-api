using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.ConnectGitHubRepository;

public sealed record ConnectGitHubRepositoryCommand(
    Guid OrganizationId,
    Guid ProjectId,
    long InstallationId,
    long RepositoryId,
    string RepositoryOwner,
    string RepositoryName) : IRequest<ErrorOr<Success>>;
