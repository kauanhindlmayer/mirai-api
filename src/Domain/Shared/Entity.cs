namespace Domain.Shared;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; protected init; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; protected set; }

    protected readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected Entity() { }
}