using Mirai.Domain.Common;

namespace Mirai.Domain.Retrospectives;

public class RetrospectiveItem : Entity
{
    public string Description { get; private set; } = null!;
    public int Votes { get; private set; }
    public Guid RetrospectiveColumnId { get; private set; }
    public RetrospectiveColumn RetrospectiveColumn { get; private set; } = null!;

    public RetrospectiveItem(string description, int votes)
    {
        Description = description;
        Votes = votes;
    }

    private RetrospectiveItem()
    {
    }
}