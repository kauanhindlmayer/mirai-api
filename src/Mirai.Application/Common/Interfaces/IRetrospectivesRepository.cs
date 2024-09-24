using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Common.Interfaces;

public interface IRetrospectivesRepository
{
    Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken = default);
    Task<Retrospective?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Retrospective retrospective, CancellationToken cancellationToken = default);
    Task RemoveAsync(Retrospective retrospective, CancellationToken cancellationToken = default);
}