using ErrorOr;

namespace Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<ErrorOr<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}