using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Boards.Persistence;

public class BoardsRepository(AppDbContext dbContext) : IBoardsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Board board, CancellationToken cancellationToken)
    {
        await _dbContext.Boards.AddAsync(board, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Boards.FirstOrDefaultAsync(board => board.Id == id, cancellationToken);
    }
}