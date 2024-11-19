using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BoardsRepository : Repository<Board>, IBoardsRepository
{
    public BoardsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public new async Task<Board?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Boards
            .Include(board => board.Columns)
                .ThenInclude(column => column.Cards)
            .FirstOrDefaultAsync(board => board.Id == id, cancellationToken);
    }
}