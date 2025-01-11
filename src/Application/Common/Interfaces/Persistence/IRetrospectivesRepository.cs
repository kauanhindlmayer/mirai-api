using Domain.Retrospectives;

namespace Application.Common.Interfaces.Persistence;

public interface IRetrospectivesRepository : IRepository<Retrospective>
{
    Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default);
}