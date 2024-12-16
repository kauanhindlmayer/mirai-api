namespace Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected readonly List<IDomainEvent> _domainEvents = [];

    public List<IDomainEvent> GetDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();
        return copy;
    }

    protected Entity() { }
}