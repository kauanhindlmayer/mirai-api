using Domain.Shared;

namespace Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdentityIdAsync(
        string identityId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByPasswordResetTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}