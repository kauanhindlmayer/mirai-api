using Application.Common.Interfaces;
using Domain.Boards;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Boards.Persistence;

public class BoardsRepository : Repository<Board>, IBoardsRepository
{
    public BoardsRepository(AppDbContext dbContext)
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