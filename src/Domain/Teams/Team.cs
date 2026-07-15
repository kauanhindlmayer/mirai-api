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
        if (Sprints.Any(s => s.Id == sprint.Id))
        {
            return SprintErrors.AlreadyExists;
        }

        var result = EnsureSprintIsDistinct(
            sprint.Id,
            sprint.Name,
            sprint.StartDate,
            sprint.EndDate);

        if (result.IsError)
        {
            return result.Errors;
        }

        Sprints.Add(sprint);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateSprint(
        Guid sprintId,
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        var sprint = Sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        var result = EnsureSprintIsDistinct(sprintId, name, startDate, endDate);
        if (result.IsError)
        {
            return result.Errors;
        }

        sprint.Update(name, startDate, endDate);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteSprint(Guid sprintId)
    {
        var sprint = Sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        sprint.ReturnWorkItemsToBacklog();
        Sprints.Remove(sprint);
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

    /// <summary>
    /// A team's sprints have distinct names and may not overlap in time. The
    /// sprint being checked is excluded, so re-saving one against itself passes.
    /// </summary>
    private ErrorOr<Success> EnsureSprintIsDistinct(
        Guid sprintId,
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        var otherSprints = Sprints.Where(s => s.Id != sprintId).ToList();

        var sprintWithSameName = otherSprints.FirstOrDefault(s => s.Name == name);
        if (sprintWithSameName is not null)
        {
            return SprintErrors.NameTakenBySprint(sprintWithSameName.Name);
        }

        var overlappingSprint = otherSprints
            .FirstOrDefault(s => s.Overlaps(startDate, endDate));

        if (overlappingSprint is not null)
        {
            return SprintErrors.OverlapsSprint(overlappingSprint.Name);
        }

        return Result.Success;
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