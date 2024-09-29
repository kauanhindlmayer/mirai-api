using Domain.Users;

namespace Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);