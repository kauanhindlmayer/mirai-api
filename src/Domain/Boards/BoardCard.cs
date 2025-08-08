using Domain.Shared;
using Domain.WorkItems;

namespace Domain.Boards;

public sealed class BoardCard : Entity
{
    public Guid BoardColumnId { get; private set; }
    public BoardColumn BoardColumn { get; private set; } = null!;
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public int Position { get; private set; }

    public BoardCard(Guid boardColumnId, Guid workItemId, int position = 0)
    {
        BoardColumnId = boardColumnId;
        WorkItemId = workItemId;
        Position = position;
    }

    private BoardCard()
    {
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }
}