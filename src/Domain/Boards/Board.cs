using Domain.Common;
using Domain.Projects;
using ErrorOr;

namespace Domain.Boards;

public class Board : AggregateRoot
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

    public ErrorOr<BoardColumn> AddColumn(string name, int wipLimit, string definitionOfDone)
    {
        if (Columns.Any(c => c.Name == name))
        {
            return BoardErrors.ColumnAlreadyExists;
        }

        var position = Columns.Count;
        var column = new BoardColumn(Id, name, position, wipLimit, definitionOfDone);
        Columns.Add(column);
        return column;
    }

    public ErrorOr<Success> RemoveColumn(Guid columnId)
    {
        var column = Columns.SingleOrDefault(c => c.Id == columnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        if (column.Cards.Count != 0)
        {
            return BoardErrors.ColumnHasCards(column);
        }

        Columns.Remove(column);
        ReorderColumns();
        return Result.Success;
    }

    private void ReorderColumns()
    {
        var position = 0;
        foreach (var column in Columns.OrderBy(c => c.Position))
        {
            column.UpdatePosition(position);
            position++;
        }
    }
}