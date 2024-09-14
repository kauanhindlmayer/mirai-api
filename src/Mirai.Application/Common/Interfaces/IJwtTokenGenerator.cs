using Mirai.Domain.Users;

namespace Mirai.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid id,
        string firstName,
        string lastName,
        string email,
        List<string> permissions,
        List<string> roles);
}