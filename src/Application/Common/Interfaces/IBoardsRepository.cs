using Domain.Boards;

namespace Application.Common.Interfaces;

public interface IBoardsRepository
{
    Task AddAsync(Board board, CancellationToken cancellationToken);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Update(Board board);
    void Remove(Board board);
}