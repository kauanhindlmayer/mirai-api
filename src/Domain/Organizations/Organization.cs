using Domain.Common;
using Domain.Organizations.Events;
using Domain.Projects;
using Domain.Users;
using ErrorOr;

namespace Domain.Organizations;

public sealed class Organization : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ICollection<Project> Projects { get; private set; } = [];
    public ICollection<User> Members { get; private set; } = [];

    public Organization(string name, string description)
    {
        Name = name;
        Description = description;
        AddDomainEvent(new OrganizationCreatedDomainEvent(this));
    }

    private Organization()
    {
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        AddDomainEvent(new OrganizationUpdatedDomainEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new OrganizationDeletedDomainEvent(this));
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

    public ErrorOr<Success> RemoveProject(Project project)
    {
        var projectToRemove = Projects.FirstOrDefault(p => p.Id == project.Id);
        if (projectToRemove is null)
        {
            return ProjectErrors.NotFound;
        }

        Projects.Remove(projectToRemove);
        return Result.Success;
    }

    public ErrorOr<Success> AddMember(User member)
    {
        if (Members.Any(m => m.Id == member.Id))
        {
            return OrganizationErrors.UserAlreadyMember;
        }

        Members.Add(member);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(User member)
    {
        var memberToRemove = Members.FirstOrDefault(m => m.Id == member.Id);
        if (memberToRemove is null)
        {
            return OrganizationErrors.UserNotMember;
        }

        Members.Remove(memberToRemove);
        return Result.Success;
    }
}