using ErrorOr;

namespace Domain.Common;

public interface IPasswordHasher
{
    public ErrorOr<string> HashPassword(string password);
    bool IsCorrectPassword(string password, string hash);
}