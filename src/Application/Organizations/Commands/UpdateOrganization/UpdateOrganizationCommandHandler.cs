using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

internal sealed class UpdateOrganizationCommandHandler(
    IOrganizationsRepository organizationsRepository)
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await organizationsRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Update(command.Name, command.Description);
        organizationsRepository.Update(organization);

        return organization;
    }
}