using Domain.Boards;

namespace Application.Common.Interfaces.Persistence;

public interface IBoardsRepository : IRepository<Board>
{
    Task<Board?> GetByIdWithColumnsAsync(Guid boardId, CancellationToken cancellationToken);
    Task<Board?> GetByIdWithCardsAsync(Guid boardId, CancellationToken cancellationToken);
}