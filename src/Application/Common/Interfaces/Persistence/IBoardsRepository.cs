using Domain.Boards;

namespace Application.Common.Interfaces.Persistence;

public interface IBoardsRepository
{
    Task AddAsync(Board board, CancellationToken cancellationToken = default);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Board board);
    void Remove(Board board);
}