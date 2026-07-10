using Domain.Authorization;
using Domain.Boards;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Shared;
using Domain.Sprints;
using Domain.Teams.Events;
using Domain.Users;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Teams;

public sealed class Team : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsDefault { get; private set; }
    public Board Board { get; private set; } = null!;
    public ICollection<TeamMember> Members { get; private set; } = [];
    public ICollection<Retrospective> Retrospectives { get; private set; } = [];
    public ICollection<Sprint> Sprints { get; private set; } = [];
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Team(Guid projectId, string name, string description, bool isDefault = false)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
        IsDefault = isDefault;
        RaiseDomainEvent(new TeamCreatedDomainEvent(this));
    }

    private Team()
    {
    }

    public ErrorOr<Success> AddMember(User user, Role role)
    {
        if (role.Scope != RoleScope.Team)
        {
            return RoleErrors.ScopeMismatch;
        }

        if (Members.Any(m => m.UserId == user.Id))
        {
            return TeamErrors.UserAlreadyExists;
        }

        Members.Add(new TeamMember(Id, user, role));
        RaiseDomainEvent(new UserAddedToTeamDomainEvent(this, user));
        return Result.Success;
    }

    public ErrorOr<Success> ChangeMemberRole(Guid userId, Role newRole)
    {
        if (newRole.Scope != RoleScope.Team)
        {
            return RoleErrors.ScopeMismatch;
        }

        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return TeamErrors.UserNotFound;
        }

        if (newRole.Id != SystemRoles.TeamAdminId && IsLastAdmin(userId))
        {
            return TeamErrors.CannotRemoveLastAdmin;
        }

        member.ChangeRole(newRole);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveUser(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return TeamErrors.UserNotFound;
        }

        if (WorkItems.Any(wi => wi.AssigneeId == userId))
        {
            return TeamErrors.UserHasAssignedWorkItems;
        }

        if (IsLastAdmin(userId))
        {
            return TeamErrors.CannotRemoveLastAdmin;
        }

        Members.Remove(member);
        RaiseDomainEvent(new UserRemovedFromTeamDomainEvent(this, member.User));
        return Result.Success;
    }

    public ErrorOr<Success> AddRetrospective(Retrospective retrospective)
    {
        if (Retrospectives.Any(r => r.Title == retrospective.Title))
        {
            return RetrospectiveErrors.AlreadyExists;
        }

        Retrospectives.Add(retrospective);
        return Result.Success;
    }

    public ErrorOr<Success> AddSprint(Sprint sprint)
    {
        if (Sprints.Any(s => s.Name == sprint.Name))
        {
            return SprintErrors.AlreadyExists;
        }

        Sprints.Add(sprint);
        return Result.Success;
    }

    public ErrorOr<Success> AddBoard(Board board)
    {
        if (Board is not null)
        {
            return TeamErrors.BoardAlreadyExists;
        }

        Board = board;
        return Result.Success;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void UnsetAsDefault()
    {
        IsDefault = false;
    }

    private bool IsLastAdmin(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null || member.RoleId != SystemRoles.TeamAdminId)
        {
            return false;
        }

        return Members.Count(m => m.RoleId == SystemRoles.TeamAdminId) == 1;
    }
}