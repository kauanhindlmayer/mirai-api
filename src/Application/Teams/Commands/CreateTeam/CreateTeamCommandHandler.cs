using Domain.Projects;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

internal sealed class CreateTeamCommandHandler
    : IRequestHandler<CreateTeamCommand, ErrorOr<Guid>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateTeamCommandHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var team = new Team(
            project.Id,
            command.Name,
            command.Description);

        var result = project.AddTeam(team);

        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return team.Id;
    }
}