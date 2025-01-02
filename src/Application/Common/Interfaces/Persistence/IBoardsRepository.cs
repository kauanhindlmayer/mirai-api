using Domain.Boards;

namespace Application.Common.Interfaces.Persistence;

public interface IBoardsRepository : IRepository<Board>
{
    Task<Board?> GetByIdWithColumnsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}