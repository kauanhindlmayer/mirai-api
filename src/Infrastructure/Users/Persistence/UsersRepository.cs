using Application.Common.Interfaces;
using Domain.Users;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Persistence;

public class UsersRepository(AppDbContext _dbContext) : IUsersRepository
{
    private readonly AppDbContext _dbContext = _dbContext;

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Update(user);
    }

    public void Remove(User user)
    {
        _dbContext.Remove(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
}