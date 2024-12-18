using Domain.Common;

namespace Application.Common.Interfaces.Persistence;

public interface IRepository<T>
    where T : Entity
{
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}