using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.UpdateProject;

internal sealed class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public UpdateProjectCommandHandler(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAsync(
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
        _organizationRepository.Update(organization);

        return project.Id;
    }
}