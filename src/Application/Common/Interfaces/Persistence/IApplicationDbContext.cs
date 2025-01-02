using Domain.Boards;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Board> Boards { get; }
}