using Application.Abstractions;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Commands.AddUserToOrganization;

internal sealed class AddUserToOrganizationCommandHandler
    : IRequestHandler<AddUserToOrganizationCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public AddUserToOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAndUsersAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var user = await _userRepository.GetByEmailAsync(
            command.Email,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        if (organization.Members.Any(m => m.UserId == user.Id))
        {
            return OrganizationErrors.UserAlreadyExists;
        }

        var memberRole = await _context.Roles
            .FirstAsync(r => r.Id == SystemRoles.OrganizationMemberId, cancellationToken);

        var result = organization.AddMember(user, memberRole);
        if (result.IsError)
        {
            return result.Errors;
        }

        _organizationRepository.Update(organization);

        return Result.Success;
    }
}