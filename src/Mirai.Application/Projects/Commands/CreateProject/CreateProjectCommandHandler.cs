using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<CreateProjectCommand, ErrorOr<Project>>
{
    public async Task<ErrorOr<Project>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return OrganizationErrors.OrganizationNotFound;
        }

        var project = new Project(
            name: request.Name,
            description: request.Description,
            organizationId: request.OrganizationId);

        var result = organization.AddProject(project);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _organizationsRepository.UpdateAsync(organization, cancellationToken);

        return project;
    }
}