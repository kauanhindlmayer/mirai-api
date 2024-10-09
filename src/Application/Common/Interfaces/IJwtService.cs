using ErrorOr;

namespace Application.Common.Interfaces;

public interface IJwtService
{
    Task<ErrorOr<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}