using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

internal sealed class UpdateOrganizationCommandHandler
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationsRepository _organizationsRepository;

    public UpdateOrganizationCommandHandler(IOrganizationsRepository organizationsRepository)
    {
        _organizationsRepository = organizationsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Update(command.Name, command.Description);
        _organizationsRepository.Update(organization);

        return organization.Id;
    }
}