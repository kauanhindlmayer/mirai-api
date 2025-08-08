using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

internal sealed class DeleteOrganizationCommandHandler
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationsRepository _organizationsRepository;

    public DeleteOrganizationCommandHandler(IOrganizationsRepository organizationsRepository)
    {
        _organizationsRepository = organizationsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Delete();
        _organizationsRepository.Remove(organization);

        return Result.Success;
    }
}