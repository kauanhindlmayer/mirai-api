using ErrorOr;

namespace Application.Common.Interfaces.Services;

public interface IJwtService
{
    Task<ErrorOr<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}