using Domain.Organizations;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.AddUserToOrganization;

internal sealed class AddUserToOrganizationCommandHandler
    : IRequestHandler<AddUserToOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    public AddUserToOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        var result = organization.AddUser(user);
        if (result.IsError)
        {
            return result.Errors;
        }

        _organizationRepository.Update(organization);

        return Result.Success;
    }
}