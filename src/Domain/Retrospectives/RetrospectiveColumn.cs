using Domain.Common;

namespace Domain.Retrospectives;

public class RetrospectiveColumn : Entity
{
    public string Title { get; private set; } = null!;
    public Guid RetrospectiveId { get; private set; }
    public Retrospective Retrospective { get; private set; } = null!;
    public List<RetrospectiveItem> Items { get; private set; } = [];

    public RetrospectiveColumn(string title, Guid retrospectiveId)
    {
        Title = title;
        RetrospectiveId = retrospectiveId;
    }

    private RetrospectiveColumn()
    {
    }

    public void AddItem(RetrospectiveItem item)
    {
        Items.Add(item);
    }
}