using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

internal sealed class DeleteOrganizationCommandHandler
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public DeleteOrganizationCommandHandler(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Delete();
        _organizationRepository.Remove(organization);

        return Result.Success;
    }
}