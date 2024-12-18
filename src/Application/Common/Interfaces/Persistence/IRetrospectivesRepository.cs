using Domain.Retrospectives;

namespace Application.Common.Interfaces.Persistence;

public interface IRetrospectivesRepository : IRepository<Retrospective>
{
    Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken = default);
}