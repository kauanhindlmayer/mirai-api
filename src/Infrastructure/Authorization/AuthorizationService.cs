using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class AuthorizationService(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<User?> GetUserAsync(string identityId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityId);
    }
}
