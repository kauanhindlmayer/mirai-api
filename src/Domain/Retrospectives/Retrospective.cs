using Domain.Retrospectives.Enums;
using Domain.Shared;
using Domain.Teams;
using ErrorOr;

namespace Domain.Retrospectives;

public sealed class Retrospective : AggregateRoot
{
    private const int DefaultVotesPerUserLimit = 5;
    private const int MaxColumns = 5;
    public string Title { get; private set; } = null!;
    public int MaxVotesPerUser { get; private set; }
    public RetrospectiveTemplate Template { get; private set; }
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
        MaxVotesPerUser = maxVotesPerUser ?? DefaultVotesPerUserLimit;
        Template = template ?? RetrospectiveTemplate.Classic;
        TeamId = teamId;
        InitializeDefaultColumns();
    }

    private Retrospective()
    {
    }

    public void Update(
        string? title = null,
        int? maxVotesPerUser = null,
        RetrospectiveTemplate? template = null)
    {
        Title = title ?? Title;
        MaxVotesPerUser = maxVotesPerUser ?? MaxVotesPerUser;

        if (template.HasValue && template.Value != Template)
        {
            Template = template.Value;
            Columns.Clear();
            InitializeDefaultColumns();
        }
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

    public ErrorOr<Success> AddItem(RetrospectiveItem item)
    {
        var column = Columns.FirstOrDefault(c => c.Id == item.RetrospectiveColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.ColumnNotFound;
        }

        column.AddItem(item);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveItem(Guid itemId)
    {
        var column = Columns.FirstOrDefault(c => c.Items.Any(i => i.Id == itemId));
        if (column is null)
        {
            return RetrospectiveErrors.ItemNotFound;
        }

        return column.RemoveItem(itemId);
    }

    private void InitializeDefaultColumns()
    {
        var columnTitles = RetrospectiveColumnTemplates.Templates[Template];
        foreach (var title in columnTitles)
        {
            AddColumn(new RetrospectiveColumn(title, Id));
        }
    }
}