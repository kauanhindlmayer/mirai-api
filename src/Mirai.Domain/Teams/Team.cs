using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Projects;
using Mirai.Domain.Retrospectives;
using Mirai.Domain.Users;

namespace Mirai.Domain.Teams;

public class Team : Entity
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ICollection<User> Members { get; private set; } = [];
    public ICollection<Retrospective> Retrospectives { get; private set; } = [];

    public Team(Guid projectId, string name)
    {
        ProjectId = projectId;
        Name = name;
    }

    private Team()
    {
    }

    public ErrorOr<Success> AddMember(User user)
    {
        if (Members.Contains(user))
        {
            return TeamErrors.TeamMemberAlreadyExists;
        }

        Members.Add(user);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(User user)
    {
        if (!Members.Contains(user))
        {
            return TeamErrors.TeamMemberNotFound;
        }

        Members.Remove(user);
        return Result.Success;
    }
}