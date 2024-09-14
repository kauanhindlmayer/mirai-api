using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Projects;

namespace Mirai.Domain.Organizations;

public class Organization : Entity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ICollection<Project> Projects { get; private set; } = [];

    public Organization(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    private Organization()
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
            return OrganizationErrors.ProjectWithSameNameAlreadyExists;
        }

        Projects.Add(project);
        return Result.Success;
    }
}