using Domain.Users;

namespace Application.Common.Interfaces.Services;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);
}