using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

internal sealed class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public CreateProjectCommandHandler(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var project = new Project(
            command.Name,
            command.Description,
            command.OrganizationId);

        var result = organization.AddProject(project);
        if (result.IsError)
        {
            return result.Errors;
        }

        _organizationRepository.Update(organization);

        return project.Id;
    }
}