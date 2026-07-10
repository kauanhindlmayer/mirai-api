using Domain.Authorization;
using Domain.Shared;
using Domain.Users;

namespace Domain.Teams;

public sealed class TeamMember : Entity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public TeamMember(Guid teamId, User user, Role role)
    {
        TeamId = teamId;
        UserId = user.Id;
        User = user;
        RoleId = role.Id;
        Role = role;
    }

    private TeamMember()
    {
    }

    public void ChangeRole(Role role)
    {
        RoleId = role.Id;
        Role = role;
    }
}
