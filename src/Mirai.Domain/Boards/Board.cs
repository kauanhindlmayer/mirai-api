using Mirai.Domain.Common;
using Mirai.Domain.Projects;

namespace Mirai.Domain.Boards;

public class Board : Entity
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public ICollection<BoardColumn> Columns { get; private set; } = [];

    public Board(Guid projectId, string name, string description)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
    }

    private Board()
    {
    }
}