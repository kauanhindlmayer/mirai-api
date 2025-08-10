using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

internal sealed class UpdateOrganizationCommandHandler
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Update(command.Name, command.Description);
        _organizationRepository.Update(organization);

        return organization.Id;
    }
}