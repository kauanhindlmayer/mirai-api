using Application.Abstractions.Authentication;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.ConnectGitHubRepository;

internal sealed class ConnectGitHubRepositoryCommandHandler
    : IRequestHandler<ConnectGitHubRepositoryCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;

    public ConnectGitHubRepositoryCommandHandler(
        IProjectRepository projectRepository,
        IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        ConnectGitHubRepositoryCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null || project.OrganizationId != command.OrganizationId)
        {
            return ProjectErrors.NotFound;
        }

        var existingConnectionOwner = await _projectRepository.GetByGitHubRepositoryIdAsync(
            command.RepositoryId,
            cancellationToken);

        if (existingConnectionOwner is not null && existingConnectionOwner.Id != project.Id)
        {
            return ProjectErrors.GitHubRepositoryAlreadyConnectedElsewhere;
        }

        var result = project.ConnectGitHubRepository(
            command.InstallationId,
            command.RepositoryId,
            command.RepositoryOwner,
            command.RepositoryName,
            _userContext.UserId);

        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return Result.Success;
    }
}
