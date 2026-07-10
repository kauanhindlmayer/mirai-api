using Domain.Authorization;
using Domain.Shared;
using Domain.Users;

namespace Domain.Organizations;

public sealed class OrganizationMember : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public OrganizationMember(Guid organizationId, User user, Role role)
    {
        OrganizationId = organizationId;
        UserId = user.Id;
        User = user;
        RoleId = role.Id;
        Role = role;
    }

    private OrganizationMember()
    {
    }

    public void ChangeRole(Role role)
    {
        RoleId = role.Id;
        Role = role;
    }
}
