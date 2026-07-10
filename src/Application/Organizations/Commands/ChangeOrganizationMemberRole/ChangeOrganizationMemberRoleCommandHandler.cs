using Application.Abstractions;
using Domain.Authorization;
using Domain.Organizations;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Commands.ChangeOrganizationMemberRole;

internal sealed class ChangeOrganizationMemberRoleCommandHandler
    : IRequestHandler<ChangeOrganizationMemberRoleCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IApplicationDbContext _context;

    public ChangeOrganizationMemberRoleCommandHandler(
        IOrganizationRepository organizationRepository,
        IApplicationDbContext context)
    {
        _organizationRepository = organizationRepository;
        _context = context;
    }

    public async Task<ErrorOr<Success>> Handle(
        ChangeOrganizationMemberRoleCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAndUsersAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == command.RoleId, cancellationToken);

        if (role is null)
        {
            return RoleErrors.NotFound;
        }

        var result = organization.ChangeMemberRole(command.UserId, role);
        if (result.IsError)
        {
            return result.Errors;
        }

        _organizationRepository.Update(organization);

        return Result.Success;
    }
}
