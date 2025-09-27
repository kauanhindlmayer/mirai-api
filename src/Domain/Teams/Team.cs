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
    public ICollection<User> Users { get; private set; } = [];
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

    public ErrorOr<Success> AddUser(User user)
    {
        if (Users.Contains(user))
        {
            return TeamErrors.UserAlreadyExists;
        }

        Users.Add(user);
        RaiseDomainEvent(new UserAddedToTeamDomainEvent(this, user));
        return Result.Success;
    }

    public ErrorOr<Success> RemoveUser(Guid userId)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return TeamErrors.UserNotFound;
        }

        if (WorkItems.Any(wi => wi.AssignedUserId == userId))
        {
            return TeamErrors.UserHasAssignedWorkItems;
        }

        Users.Remove(user);
        RaiseDomainEvent(new UserRemovedFromTeamDomainEvent(this, user));
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
}