using Domain.Shared;

namespace Domain.Retrospectives;

public interface IRetrospectivesRepository : IRepository<Retrospective>
{
    Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default);
}