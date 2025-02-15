using Domain.Common;
using Domain.Retrospectives.Enums;
using Domain.Teams;
using ErrorOr;

namespace Domain.Retrospectives;

public sealed class Retrospective : AggregateRoot
{
    private const int DefaultVotesPerUserLimit = 5;
    private const int MaxColumns = 5;
    public string Title { get; private set; } = null!;
    public int MaxVotesPerUser { get; private set; }
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public ICollection<RetrospectiveColumn> Columns { get; set; } = [];

    public Retrospective(
        string title,
        int? maxVotesPerUser,
        RetrospectiveTemplate? template,
        Guid teamId)
    {
        Title = title;
        MaxVotesPerUser = maxVotesPerUser.GetValueOrDefault(DefaultVotesPerUserLimit);
        TeamId = teamId;
        InitializeDefaultColumns(template ?? RetrospectiveTemplate.Classic);
    }

    private Retrospective()
    {
    }

    public ErrorOr<Success> AddColumn(RetrospectiveColumn column)
    {
        if (Columns.Count >= MaxColumns)
        {
            return RetrospectiveErrors.MaxColumnsReached;
        }

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