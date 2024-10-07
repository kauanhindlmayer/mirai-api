using Domain.Common;
using Domain.Teams;
using ErrorOr;

namespace Domain.Retrospectives;

public class Retrospective : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public ICollection<RetrospectiveColumn> Columns { get; set; } = [];

    public Retrospective(string title, string description, Guid teamId)
    {
        Title = title;
        Description = description;
        TeamId = teamId;
    }

    private Retrospective()
    {
    }

    public ErrorOr<Success> AddColumn(RetrospectiveColumn column)
    {
        if (Columns.Any(c => c.Title == column.Title))
        {
            return RetrospectiveErrors.ColumnAlreadyExists;
        }

        column.UpdatePosition(Columns.Count);
        Columns.Add(column);
        return Result.Success;
    }
}