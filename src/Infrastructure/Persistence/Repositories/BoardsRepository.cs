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

    public Task<Board?> GetByIdWithColumnsAsync(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Boards
            .Include(b => b.Columns)
            .SingleOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }

    public Task<Board?> GetByIdWithCardsAsync(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Boards
            .Include(b => b.Team)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Cards)
            .SingleOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }
}