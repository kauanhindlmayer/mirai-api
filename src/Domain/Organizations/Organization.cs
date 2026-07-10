using Domain.Authorization;
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
    public ICollection<OrganizationMember> Members { get; private set; } = [];

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

    public ErrorOr<Success> AddMember(User user, Role role)
    {
        if (role.Scope != RoleScope.Organization)
        {
            return RoleErrors.ScopeMismatch;
        }

        if (Members.Any(m => m.UserId == user.Id))
        {
            return OrganizationErrors.UserAlreadyExists;
        }

        Members.Add(new OrganizationMember(Id, user, role));
        RaiseDomainEvent(new UserAddedToOrganizationDomainEvent(this, user));
        return Result.Success;
    }

    public ErrorOr<Success> ChangeMemberRole(Guid userId, Role newRole)
    {
        if (newRole.Scope != RoleScope.Organization)
        {
            return RoleErrors.ScopeMismatch;
        }

        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return UserErrors.NotFound;
        }

        if (newRole.Id != SystemRoles.OrganizationOwnerId && IsLastOwner(userId))
        {
            return OrganizationErrors.CannotRemoveLastOwner;
        }

        member.ChangeRole(newRole);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveUser(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return UserErrors.NotFound;
        }

        if (Projects.Any(p => p.Members.Any(m => m.UserId == userId)))
        {
            return OrganizationErrors.UserHasProjects;
        }

        if (IsLastOwner(userId))
        {
            return OrganizationErrors.CannotRemoveLastOwner;
        }

        Members.Remove(member);
        RaiseDomainEvent(new UserRemovedFromOrganizationDomainEvent(this, member.User));
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

    private bool IsLastOwner(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null || member.RoleId != SystemRoles.OrganizationOwnerId)
        {
            return false;
        }

        return Members.Count(m => m.RoleId == SystemRoles.OrganizationOwnerId) == 1;
    }
}