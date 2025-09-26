using Domain.Shared;

namespace Domain.Retrospectives;

public interface IRetrospectiveRepository : IRepository<Retrospective>
{
    Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default);
}