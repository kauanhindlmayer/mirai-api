using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Projects;

namespace Mirai.Domain.Retrospectives;

public class Retrospective : Entity
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public ICollection<RetrospectiveColumn> Columns { get; set; } = [];

    public Retrospective(string title, string description, Guid projectId)
    {
        Title = title;
        Description = description;
        ProjectId = projectId;
    }

    private Retrospective()
    {
    }

    public void AddColumn(RetrospectiveColumn column)
    {
        Columns.Add(column);
    }

    public ErrorOr<Success> AddItem(RetrospectiveItem item, Guid columnId)
    {
        var column = Columns.FirstOrDefault(c => c.Id == columnId);
        if (column is null)
        {
            return RetrospectiveErrors.RetrospectiveColumnNotFound;
        }

        column.AddItem(item);
        return Result.Success;
    }
}