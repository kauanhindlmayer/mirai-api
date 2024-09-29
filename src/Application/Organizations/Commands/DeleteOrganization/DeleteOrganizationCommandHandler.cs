using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler(
    IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.OrganizationNotFound;
        }

        _organizationsRepository.Remove(organization);

        return Result.Success;
    }
}