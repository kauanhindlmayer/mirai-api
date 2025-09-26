using Domain.Shared;

namespace Domain.Boards;

public interface IBoardRepository : IRepository<Board>
{
    Task<Board?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken);
    Task<Board?> GetByIdWithCardsAsync(Guid id, CancellationToken cancellationToken);
}