using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

internal sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;

    public CreateTeamCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
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

        _projectsRepository.Update(project);

        return team.Id;
    }
}