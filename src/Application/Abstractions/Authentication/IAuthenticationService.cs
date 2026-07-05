using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        string identityId,
        string newPassword,
        CancellationToken cancellationToken = default);
}