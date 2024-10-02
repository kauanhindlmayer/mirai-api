using Domain.Retrospectives;

namespace Application.Common.Interfaces;

public interface IRetrospectivesRepository
{
    Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken = default);
    Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken = default);
    void Update(Retrospective retrospective);
    void Remove(Retrospective retrospective);
}