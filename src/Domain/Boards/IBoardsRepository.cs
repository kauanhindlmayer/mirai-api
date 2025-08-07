using Domain.Common;

namespace Domain.Boards;

public interface IBoardsRepository : IRepository<Board>
{
    Task<Board?> GetByIdWithColumnsAsync(Guid boardId, CancellationToken cancellationToken);
    Task<Board?> GetByIdWithCardsAsync(Guid boardId, CancellationToken cancellationToken);
}