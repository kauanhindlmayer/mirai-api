using Domain.Users;

namespace Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Update(User user);
    void Remove(User user);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}