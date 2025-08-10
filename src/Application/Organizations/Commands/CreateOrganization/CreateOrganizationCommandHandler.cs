using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

internal sealed class CreateOrganizationCommandHandler
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public CreateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        if (await _organizationRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken))
        {
            return OrganizationErrors.AlreadyExists;
        }

        var organization = new Organization(
            command.Name,
            command.Description);

        await _organizationRepository.AddAsync(
            organization,
            cancellationToken);

        return organization.Id;
    }
}