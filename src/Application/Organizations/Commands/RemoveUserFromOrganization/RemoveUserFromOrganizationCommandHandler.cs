using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.RemoveUserFromOrganization;

internal sealed class RemoveUserFromOrganizationCommandHandler
    : IRequestHandler<RemoveUserFromOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationsRepository _organizationsRepository;

    public RemoveUserFromOrganizationCommandHandler(
        IOrganizationsRepository organizationsRepository)
    {
        _organizationsRepository = organizationsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveUserFromOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdWithProjectsAndUsersAsync(
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

        _organizationsRepository.Update(organization);

        return Result.Success;
    }
}