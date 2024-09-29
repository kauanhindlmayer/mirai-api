using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.OrganizationNotFound;
        }

        organization.Update(request.Name, request.Description);
        _organizationsRepository.Update(organization);

        return organization;
    }
}