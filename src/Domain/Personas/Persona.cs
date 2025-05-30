using Domain.Common;
using Domain.Projects;

namespace Domain.Personas;

public sealed class Persona : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid? ImageFileId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public Persona(Guid projectId, string name, string? description)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
    }

    private Persona()
    {
    }

    public void SetImage(string? imageUrl, Guid? imageFileId)
    {
        ImageUrl = imageUrl;
        ImageFileId = imageFileId;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}