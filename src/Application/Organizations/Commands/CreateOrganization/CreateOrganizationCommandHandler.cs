using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        if (await _organizationsRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return OrganizationErrors.OrganizationWithSameNameAlreadyExists;
        }

        var organization = new Organization(
            request.Name,
            request.Description);

        await _organizationsRepository.AddAsync(organization, cancellationToken);

        return organization;
    }
}