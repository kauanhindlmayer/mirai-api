using Domain.Organizations.Events;
using Domain.Projects;
using Domain.Shared;
using Domain.Users;
using ErrorOr;

namespace Domain.Organizations;

public sealed class Organization : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ICollection<Project> Projects { get; private set; } = [];
    public ICollection<User> Users { get; private set; } = [];

    public Organization(string name, string description)
    {
        Name = name;
        Description = description;
        RaiseDomainEvent(new OrganizationCreatedDomainEvent(this));
    }

    private Organization()
    {
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        RaiseDomainEvent(new OrganizationUpdatedDomainEvent(this));
    }

    public void Delete()
    {
        RaiseDomainEvent(new OrganizationDeletedDomainEvent(this));
    }

    public ErrorOr<Success> AddUser(User user)
    {
        if (Users.Any(u => u.Id == user.Id))
        {
            return OrganizationErrors.UserAlreadyExists;
        }

        Users.Add(user);
        RaiseDomainEvent(new UserAddedToOrganizationDomainEvent(this, user));
        return Result.Success;
    }

    public ErrorOr<Success> RemoveUser(Guid userId)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return UserErrors.NotFound;
        }

        if (Projects.Any(p => p.Users.Any(u => u.Id == userId)))
        {
            return OrganizationErrors.UserHasProjects;
        }

        Users.Remove(user);
        RaiseDomainEvent(new UserRemovedFromOrganizationDomainEvent(this, user));
        return Result.Success;
    }

    public ErrorOr<Success> AddProject(Project project)
    {
        if (Projects.Any(p => p.Name == project.Name))
        {
            return ProjectErrors.AlreadyExists;
        }

        Projects.Add(project);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveProject(Guid projectId)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        Projects.Remove(project);
        return Result.Success;
    }
}