using Domain.Users;

namespace Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default);
}