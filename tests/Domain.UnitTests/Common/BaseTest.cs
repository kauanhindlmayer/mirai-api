using Domain.Common;

namespace Domain.UnitTests.Common;

public abstract class BaseTest
{
    protected static T AssertDomainEventWasPublished<T>(Entity entity)
    {
        var domainEvent = entity.GetDomainEvents().OfType<T>().SingleOrDefault();
        if (domainEvent is null)
        {
            throw new Exception($"{typeof(T).Name} was not published");
        }

        return domainEvent;
    }
}