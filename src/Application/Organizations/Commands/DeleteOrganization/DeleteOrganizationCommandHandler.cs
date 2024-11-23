using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

internal sealed class DeleteOrganizationCommandHandler(
    IOrganizationsRepository organizationsRepository)
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await organizationsRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Delete();
        organizationsRepository.Remove(organization);

        return Result.Success;
    }
}