using Application.Common.Interfaces;
using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<UpdateProjectCommand, ErrorOr<Project>>
{
    public async Task<ErrorOr<Project>> Handle(
        UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdWithProjectsAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var project = organization.Projects.FirstOrDefault(x => x.Id == command.ProjectId);
        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        project.Update(command.Name, command.Description);
        _organizationsRepository.Update(organization);

        return project;
    }
}