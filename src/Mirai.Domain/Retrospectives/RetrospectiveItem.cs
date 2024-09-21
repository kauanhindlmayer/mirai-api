using Mirai.Domain.Common;
using Mirai.Domain.Users;

namespace Mirai.Domain.Retrospectives;

public class RetrospectiveItem : Entity
{
    public string Description { get; private set; } = null!;
    public int Votes { get; private set; }
    public Guid RetrospectiveColumnId { get; private set; }
    public RetrospectiveColumn RetrospectiveColumn { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;

    public RetrospectiveItem(string description, int votes, Guid retrospectiveColumnId, Guid authorId)
    {
        Description = description;
        Votes = votes;
        RetrospectiveColumnId = retrospectiveColumnId;
        AuthorId = authorId;
    }

    private RetrospectiveItem()
    {
    }
}