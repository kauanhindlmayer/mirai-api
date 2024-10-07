using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor contextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public Guid UserId =>
        _contextAccessor
            .HttpContext?.User
            .GetUserId() ?? throw new ApplicationException("User context is unavailable");

    public string IdentityId =>
        _contextAccessor
            .HttpContext?.User
            .GetIdentityId() ?? throw new ApplicationException("User context is unavailable");
}
