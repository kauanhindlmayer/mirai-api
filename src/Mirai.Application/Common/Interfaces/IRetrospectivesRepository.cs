using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Common.Interfaces;

public interface IRetrospectivesRepository
{
    Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken = default);
    Task<Retrospective?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken = default);
    void Update(Retrospective retrospective);
    void Remove(Retrospective retrospective);
}