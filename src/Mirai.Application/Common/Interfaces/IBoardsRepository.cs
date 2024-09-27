using Mirai.Domain.Boards;

namespace Mirai.Application.Common.Interfaces;

public interface IBoardsRepository
{
    Task AddAsync(Board board, CancellationToken cancellationToken);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}