using Application.Abstractions.Authentication;
using Domain.Organizations;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

internal sealed class CreateOrganizationCommandHandler
    : IRequestHandler<CreateOrganizationCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CreateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        if (await _organizationRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken))
        {
            return OrganizationErrors.AlreadyExists;
        }

        var currentUser = await _userRepository.GetByIdAsync(
            _userContext.UserId,
            cancellationToken);

        if (currentUser is null)
        {
            return UserErrors.NotFound;
        }

        var organization = new Organization(
            command.Name,
            command.Description);

        var addUserResult = organization.AddUser(currentUser);
        if (addUserResult.IsError)
        {
            return addUserResult.Errors;
        }

        await _organizationRepository.AddAsync(
            organization,
            cancellationToken);

        return organization.Id;
    }
}