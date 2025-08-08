using Domain.Shared;
using ErrorOr;

namespace Domain.Retrospectives;

public sealed class RetrospectiveColumn : Entity
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
        if (Items.Any(i => i.Content == item.Content))
        {
            return RetrospectiveErrors.ItemAlreadyExists;
        }

        ShiftItems(0, 1);
        item.UpdatePosition(0);
        Items.Add(item);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return RetrospectiveErrors.ItemNotFound;
        }

        Items.Remove(item);
        ShiftItems(item.Position, -1);
        return Result.Success;
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }

    /// <summary>
    /// Shifts the positions of items in the column.
    /// This is used when adding or removing items to maintain the correct order.
    /// </summary>
    /// <param name="fromPosition">The position from which to start shifting.</param>
    /// <param name="offset">The amount to shift the positions by.</param>
    private void ShiftItems(int fromPosition, int offset)
    {
        foreach (var item in Items.Where(i => i.Position >= fromPosition))
        {
            item.UpdatePosition(item.Position + offset);
        }
    }
}