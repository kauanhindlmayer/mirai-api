using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.RemoveUserFromOrganization;

internal sealed class RemoveUserFromOrganizationCommandHandler
    : IRequestHandler<RemoveUserFromOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public RemoveUserFromOrganizationCommandHandler(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveUserFromOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAndUsersAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var result = organization.RemoveUser(command.UserId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _organizationRepository.Update(organization);

        return Result.Success;
    }
}