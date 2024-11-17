using Domain.Common;
using Domain.Projects;
using Domain.Users;
using ErrorOr;

namespace Domain.Organizations;

public class Organization : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ICollection<Project> Projects { get; private set; } = [];
    public ICollection<User> Members { get; private set; } = [];

    public Organization(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public Organization()
    {
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
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