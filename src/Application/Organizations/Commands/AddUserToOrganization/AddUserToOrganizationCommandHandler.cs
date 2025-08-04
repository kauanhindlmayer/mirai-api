using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.AddUserToOrganization;

internal sealed class AddUserToOrganizationCommandHandler
    : IRequestHandler<AddUserToOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationsRepository _organizationsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddUserToOrganizationCommandHandler(
        IOrganizationsRepository organizationsRepository,
        IUsersRepository usersRepository)
    {
        _organizationsRepository = organizationsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var user = await _usersRepository.GetByIdAsync(
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

        _organizationsRepository.Update(organization);

        return Result.Success;
    }
}