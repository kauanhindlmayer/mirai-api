using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organization is null)
        {
            return Error.NotFound(description: "Organization not found");
        }

        organization.Update(request.Name, request.Description);
        await _organizationsRepository.UpdateAsync(organization, cancellationToken);

        return organization;
    }
}