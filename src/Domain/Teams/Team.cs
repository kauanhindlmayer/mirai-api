using System.Collections;
using Domain.Boards;
using Domain.Common;
using Domain.Projects;
using Domain.Retrospectives;
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
    public string Description { get; private set; } = string.Empty;
    public Board Board { get; private set; } = null!;
    public ICollection<User> Members { get; private set; } = [];
    public ICollection<Retrospective> Retrospectives { get; private set; } = [];
    public ICollection<Sprint> Sprints { get; private set; } = [];
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Team(Guid projectId, string name, string description)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
        AddDomainEvent(new TeamCreatedDomainEvent(this));
    }

    private Team()
    {
    }

    public ErrorOr<Success> AddMember(User user)
    {
        if (Members.Contains(user))
        {
            return TeamErrors.MemberAlreadyExists;
        }

        Members.Add(user);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(User user)
    {
        if (!Members.Contains(user))
        {
            return TeamErrors.MemberNotFound;
        }

        Members.Remove(user);
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
}