using Domain.Common;
using Domain.Retrospectives.Enums;
using Domain.Teams;
using ErrorOr;

namespace Domain.Retrospectives;

public sealed class Retrospective : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public ICollection<RetrospectiveColumn> Columns { get; set; } = [];

    public Retrospective(
        string title,
        string description,
        Guid teamId,
        RetrospectiveTemplate? template)
    {
        Title = title;
        Description = description;
        TeamId = teamId;
        InitializeDefaultColumns(template ?? RetrospectiveTemplate.Classic);
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

    private void InitializeDefaultColumns(RetrospectiveTemplate template)
    {
        var columnTitles = RetrospectiveColumnTemplates.Templates[template];
        foreach (var title in columnTitles)
        {
            AddColumn(new RetrospectiveColumn(title, Id));
        }
    }
}