using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

internal sealed class CreateTeamCommandHandler(
    IProjectsRepository projectsRepository)
    : IRequestHandler<CreateTeamCommand, ErrorOr<Team>>
{
    public async Task<ErrorOr<Team>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var team = new Team(project.Id, command.Name);
        var addTeamResult = project.AddTeam(team);

        if (addTeamResult.IsError)
        {
            return addTeamResult.Errors;
        }

        projectsRepository.Update(project);

        return team;
    }
}