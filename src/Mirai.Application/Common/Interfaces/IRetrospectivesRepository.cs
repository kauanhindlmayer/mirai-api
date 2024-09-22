using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Common.Interfaces;

public interface IRetrospectivesRepository
{
    Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken);
    Task<Retrospective?> GetByIdAsync(Guid retrospectiveId, CancellationToken cancellationToken);
    Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken);
    Task UpdateAsync(Retrospective retrospective, CancellationToken cancellationToken);
    Task RemoveAsync(Retrospective retrospective, CancellationToken cancellationToken);
}