using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler(
    IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        if (await _organizationsRepository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return OrganizationErrors.AlreadyExists;
        }

        var organization = new Organization(
            command.Name,
            command.Description);

        await _organizationsRepository.AddAsync(organization, cancellationToken);

        return organization;
    }
}