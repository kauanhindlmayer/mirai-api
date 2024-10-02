using Domain.Common;
using Domain.Users;

namespace Domain.Retrospectives;

public class RetrospectiveItem : Entity
{
    public string Description { get; private set; } = null!;
    public int Position { get; private set; }
    public int Votes { get; private set; }
    public Guid RetrospectiveColumnId { get; private set; }
    public RetrospectiveColumn RetrospectiveColumn { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;

    public RetrospectiveItem(
        string description,
        Guid retrospectiveColumnId,
        Guid authorId)
    {
        Description = description;
        RetrospectiveColumnId = retrospectiveColumnId;
        AuthorId = authorId;
        Votes = 0;
    }

    private RetrospectiveItem()
    {
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }
}