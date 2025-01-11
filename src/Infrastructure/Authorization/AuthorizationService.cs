using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _context;

    public AuthorizationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserAsync(string identityId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.IdentityId == identityId);
    }
}
