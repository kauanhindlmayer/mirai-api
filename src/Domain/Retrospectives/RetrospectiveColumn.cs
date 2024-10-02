using Domain.Common;
using ErrorOr;

namespace Domain.Retrospectives;

public class RetrospectiveColumn : Entity
{
    public string Title { get; private set; } = null!;
    public int Position { get; private set; }
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

    public ErrorOr<Success> AddItem(RetrospectiveItem item)
    {
        if (Items.Any(i => i.Description == item.Description))
        {
            return RetrospectiveErrors.RetrospectiveItemAlreadyExists;
        }

        item.UpdatePosition(Items.Count);
        Items.Add(item);
        return Result.Success;
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }
}