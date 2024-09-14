using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Error.NotFound(description: $"Organization not found");
        }

        await _organizationsRepository.RemoveAsync(organization, cancellationToken);

        return Result.Success;
    }
}