using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class BoardsRepository : Repository<Board>, IBoardsRepository
{
    public BoardsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Board?> GetByIdWithColumnsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Boards
            .Include(board => board.Columns)
                .ThenInclude(column => column.Cards)
                    .ThenInclude(card => card.WorkItem)
                        .ThenInclude(workItem => workItem.Assignee)
            .FirstOrDefaultAsync(board => board.Id == id, cancellationToken);
    }
}