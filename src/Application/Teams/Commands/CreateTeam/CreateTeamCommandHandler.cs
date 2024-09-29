using Application.Common.Interfaces;
using Domain.Projects;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateTeamCommand, ErrorOr<Team>>
{
    public async Task<ErrorOr<Team>> Handle(
        CreateTeamCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var team = new Team(project.Id, request.Name);
        var addTeamResult = project.AddTeam(team);

        if (addTeamResult.IsError)
        {
            return addTeamResult.Errors;
        }

        _projectsRepository.Update(project);

        return team;
    }
}