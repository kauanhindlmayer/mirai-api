using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    Task<string> RegisterWithGoogleAsync(
        User user,
        string googleIdToken,
        CancellationToken cancellationToken = default);
}