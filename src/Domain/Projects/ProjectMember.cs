using Domain.Authorization;
using Domain.Shared;
using Domain.Users;

namespace Domain.Projects;

public sealed class ProjectMember : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public ProjectMember(Guid projectId, User user, Role role)
    {
        ProjectId = projectId;
        UserId = user.Id;
        User = user;
        RoleId = role.Id;
        Role = role;
    }

    private ProjectMember()
    {
    }

    public void ChangeRole(Role role)
    {
        RoleId = role.Id;
        Role = role;
    }
}
