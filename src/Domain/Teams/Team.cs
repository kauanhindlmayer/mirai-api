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

        if (sprint.Status == SprintStatus.Completed)
        {
            return SprintErrors.IsCompleted;
        }

        // An active sprint's start date is locked so the record of when it really
        // began stays honest; only its name and end date may change. A planned
        // sprint may still move freely.
        var effectiveStartDate = sprint.Status == SprintStatus.Active
            ? sprint.StartDate
            : startDate;

        var result = EnsureSprintIsDistinct(sprintId, name, effectiveStartDate, endDate);
        if (result.IsError)
        {
            return result.Errors;
        }

        sprint.Update(name, effectiveStartDate, endDate);
        return Result.Success;
    }

    /// <summary>
    /// A team runs one sprint at a time, so starting is a team-level decision:
    /// only the root can see whether a sibling sprint is already active.
    /// </summary>
    public ErrorOr<Success> StartSprint(Guid sprintId)
    {
        var sprint = Sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        if (sprint.Status != SprintStatus.Planned)
        {
            return SprintErrors.NotPlanned;
        }

        var activeSprint = Sprints.FirstOrDefault(s => s.Status == SprintStatus.Active);
        if (activeSprint is not null)
        {
            return SprintErrors.TeamAlreadyHasActiveSprint(activeSprint.Name);
        }

        sprint.Start();
        return Result.Success;
    }

    /// <summary>
    /// Completing the active sprint triages every incomplete item in one atomic
    /// act: each disposition is validated, then all are applied, then the
    /// delivered figures are frozen and the sprint becomes Completed - so a
    /// completed sprint never contains an un-triaged item.
    /// </summary>
    public ErrorOr<Success> CompleteSprint(
        Guid sprintId,
        IReadOnlyCollection<SprintItemDisposition> dispositions)
    {
        var sprint = Sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        if (sprint.Status != SprintStatus.Active)
        {
            return SprintErrors.NotActive;
        }

        var incompleteItems = sprint.IncompleteWorkItems();
        var dispositionByItem = dispositions
            .GroupBy(d => d.WorkItemId)
            .ToDictionary(g => g.Key, g => g.First());

        if (incompleteItems.Any(wi => !dispositionByItem.ContainsKey(wi.Id)))
        {
            return SprintErrors.DispositionsIncomplete;
        }

        var validation = EnsureCarryOverTargetsArePlanned(incompleteItems, dispositionByItem);
        if (validation.IsError)
        {
            return validation.Errors;
        }

        foreach (var workItem in incompleteItems)
        {
            ApplyDisposition(sprint, workItem, dispositionByItem[workItem.Id]);
        }

        sprint.Complete();
        return Result.Success;
    }

    public ErrorOr<Success> DeleteSprint(Guid sprintId)
    {
        var sprint = Sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        if (sprint.Status != SprintStatus.Planned)
        {
            return SprintErrors.CannotDeleteAfterStart;
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

    /// <summary>
    /// Validates every carry-over target before any item is moved, so a completion
    /// that will fail leaves the aggregate untouched rather than half-applied.
    /// </summary>
    private ErrorOr<Success> EnsureCarryOverTargetsArePlanned(
        IReadOnlyList<WorkItem> incompleteItems,
        IReadOnlyDictionary<Guid, SprintItemDisposition> dispositionByItem)
    {
        foreach (var workItem in incompleteItems)
        {
            var disposition = dispositionByItem[workItem.Id];
            if (disposition.Target != DispositionTarget.Sprint)
            {
                continue;
            }

            var target = Sprints.FirstOrDefault(s => s.Id == disposition.TargetSprintId);
            if (target is null || target.Status != SprintStatus.Planned)
            {
                return SprintErrors.InvalidCarryOverTarget;
            }
        }

        return Result.Success;
    }

    private void ApplyDisposition(
        Sprint sprint,
        WorkItem workItem,
        SprintItemDisposition disposition)
    {
        sprint.RemoveWorkItem(workItem);
        if (disposition.Target == DispositionTarget.Backlog)
        {
            workItem.RemoveFromSprint();
            return;
        }

        var target = Sprints.First(s => s.Id == disposition.TargetSprintId);
        target.AddWorkItem(workItem);
        workItem.AssignToSprint(target.Id);
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