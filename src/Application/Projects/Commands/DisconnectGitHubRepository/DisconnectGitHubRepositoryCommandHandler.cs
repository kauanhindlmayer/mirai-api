using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.DisconnectGitHubRepository;

internal sealed class DisconnectGitHubRepositoryCommandHandler
    : IRequestHandler<DisconnectGitHubRepositoryCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;

    public DisconnectGitHubRepositoryCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DisconnectGitHubRepositoryCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null || project.OrganizationId != command.OrganizationId)
        {
            return ProjectErrors.NotFound;
        }

        var result = project.DisconnectGitHubRepository();
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return Result.Success;
    }
}
