using Domain.Users;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class AuthorizationService(AppDbContext dbContext)
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<User?> GetUserAsync(string identityId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityId);
    }
}
