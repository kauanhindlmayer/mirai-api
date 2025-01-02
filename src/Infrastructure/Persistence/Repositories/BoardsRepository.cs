using Application.Common.Interfaces.Persistence;
using Domain.Boards;

namespace Infrastructure.Persistence.Repositories;

internal sealed class BoardsRepository : Repository<Board>, IBoardsRepository
{
    public BoardsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}