using Mirai.Domain.Users;

namespace Mirai.Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);