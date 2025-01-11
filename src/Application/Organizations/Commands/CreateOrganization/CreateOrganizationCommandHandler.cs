using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

internal sealed class CreateOrganizationCommandHandler
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationsRepository _organizationsRepository;

    public CreateOrganizationCommandHandler(IOrganizationsRepository organizationsRepository)
    {
        _organizationsRepository = organizationsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
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

        return organization.Id;
    }
}